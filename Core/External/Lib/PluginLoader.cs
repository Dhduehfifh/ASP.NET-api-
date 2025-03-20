using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace External.Lib
{
    public class PluginLoader
    {
        private IntPtr _handle;
        private readonly string _pluginPath;
        private readonly bool _isDynamic;
        private readonly Dictionary<string, Delegate> _functions = new();

        public PluginLoader(string pluginPath, bool isDynamic = true)
        {
            _pluginPath = pluginPath;
            _isDynamic = isDynamic;
        }

        /// <summary>
        /// 🌍 加载插件（默认不使用 Config，路径用户自己输入）
        /// </summary>
        public void Load()
        {
            if (_isDynamic)
            {
                _handle = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? LoadLibrary(_pluginPath) :
                          RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? dlopen(_pluginPath, RTLD_NOW) :
                          RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? dlopen(_pluginPath, RTLD_NOW) : throw new NotSupportedException("Unsupported OS");

                if (_handle == IntPtr.Zero)
                    throw new Exception($"❌ 无法加载插件: {_pluginPath}");
            }
            else
            {
                Console.WriteLine($"✅ 加载静态插件: {_pluginPath}");
            }
        }

        /// <summary>
        /// 🌍 绑定函数（用户需要自己定义接口）
        /// </summary>
        public PluginLoader BindFunction<T>(string functionName) where T : Delegate
        {
            if (!_isDynamic) throw new InvalidOperationException("❌ 静态库不支持动态调用函数");

            IntPtr functionPtr = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? GetProcAddress(_handle, functionName) :
                                 dlsym(_handle, functionName);

            if (functionPtr == IntPtr.Zero)
                throw new Exception($"❌ 未找到函数: {functionName}");

            var del = Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
            _functions[functionName] = del;
            return this;
        }

        /// <summary>
        /// 🌍 调用已绑定的函数
        /// </summary>
        public T Call<T>(string functionName, params object[] args)
        {
            if (!_functions.ContainsKey(functionName))
                throw new Exception($"❌ 函数 `{functionName}` 未绑定，请先调用 `BindFunction()`");

            return (T)_functions[functionName].DynamicInvoke(args);
        }

        /// <summary>
        /// 🌍 释放插件
        /// </summary>
        public void Unload()
        {
            if (_isDynamic)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) FreeLibrary(_handle);
                else dlclose(_handle);
                Console.WriteLine($"🔌 已卸载插件: {_pluginPath}");
            }
        }

        // Windows
        [DllImport("kernel32")] private static extern IntPtr LoadLibrary(string path);
        [DllImport("kernel32")] private static extern bool FreeLibrary(IntPtr handle);
        [DllImport("kernel32")] private static extern IntPtr GetProcAddress(IntPtr handle, string name);

        // Linux/macOS
        private const int RTLD_NOW = 2;
        [DllImport("libdl")] private static extern IntPtr dlopen(string path, int flag);
        [DllImport("libdl")] private static extern int dlclose(IntPtr handle);
        [DllImport("libdl")] private static extern IntPtr dlsym(IntPtr handle, string name);
    }
}