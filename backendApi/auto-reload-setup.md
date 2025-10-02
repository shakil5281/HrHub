# Auto-Reload Setup for .NET Application

## 🚀 **Method 1: Using `dotnet watch` (Recommended)**

### **Basic Usage:**
```bash
# Instead of: dotnet run
# Use this for auto-reload:
dotnet watch
```

### **Advanced Options:**
```bash
# Run with specific configuration
dotnet watch --configuration Release

# Run with verbose output to see what files are being watched
dotnet watch --verbose

# Run without hot reload (full restart on changes)
dotnet watch --no-hot-reload

# List all files being watched without starting
dotnet watch --list
```

### **What it does:**
- ✅ Automatically restarts the application when code changes
- ✅ Watches for changes in `.cs`, `.razor`, `.cshtml`, and other relevant files
- ✅ Supports hot reload for supported scenarios
- ✅ Works with all .NET project types

---

## 🔥 **Method 2: Hot Reload (Built-in .NET 6+)**

### **Enable Hot Reload:**
```bash
# Hot reload is enabled by default with dotnet watch
dotnet watch

# Or explicitly enable it
dotnet watch --hot-reload
```

### **What Hot Reload supports:**
- ✅ Method body changes
- ✅ Adding new methods, properties, fields
- ✅ CSS changes in real-time
- ✅ Razor page changes
- ❌ Structural changes (new classes, interfaces)

---

## ⚙️ **Method 3: Configure in launchSettings.json**

Add this to your `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "HrHubAPI-Watch": {
      "commandName": "Executable",
      "executablePath": "dotnet",
      "commandLineArgs": "watch --launch-profile HrHubAPI",
      "workingDirectory": "$(ProjectDir)",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

---

## 🐳 **Method 4: For Docker Development**

If using Docker, set this environment variable:
```bash
DOTNET_USE_POLLING_FILE_WATCHER=1
```

---

## 📝 **Method 5: Custom Watch Configuration**

Create a `.filewatcher` file in your project root to customize what files to watch:

```xml
<Project>
  <ItemGroup>
    <Watch Include="**/*.cs" />
    <Watch Include="**/*.json" />
    <Watch Include="**/*.sql" />
    <Watch Remove="bin/**/*" />
    <Watch Remove="obj/**/*" />
  </ItemGroup>
</Project>
```

---

## 🎯 **Recommended Setup for Your Project**

### **For Development:**
```bash
# Use this command for daily development
dotnet watch --verbose
```

### **For Production-like Testing:**
```bash
# Test with Release configuration
dotnet watch --configuration Release --no-hot-reload
```

---

## 🔧 **IDE Integration**

### **Visual Studio:**
- Hot Reload is built-in when debugging
- Use "Start without Debugging" (Ctrl+F5) for better hot reload experience

### **VS Code:**
- Install "C# Dev Kit" extension
- Use integrated terminal with `dotnet watch`

### **JetBrains Rider:**
- Built-in hot reload support
- Configure in Run/Debug configurations

---

## 🚨 **Troubleshooting**

### **If auto-reload isn't working:**

1. **Check file system permissions**
2. **For network drives or Docker:**
   ```bash
   export DOTNET_USE_POLLING_FILE_WATCHER=1
   dotnet watch
   ```

3. **Clear build cache:**
   ```bash
   dotnet clean
   dotnet watch
   ```

4. **Check what files are being watched:**
   ```bash
   dotnet watch --list
   ```

---

## ⚡ **Performance Tips**

- Use `--no-restore` if you don't need package restore on every change
- Exclude unnecessary directories from watching
- Use hot reload instead of full restart when possible
- Consider using `--quiet` to reduce console output

---

## 🎉 **Quick Start Command**

**Run this command in your project directory:**
```bash
dotnet watch --verbose
```

This will:
- ✅ Start your application
- ✅ Watch for file changes
- ✅ Automatically restart on changes
- ✅ Show detailed output of what's happening
