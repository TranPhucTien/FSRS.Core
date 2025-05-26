<#
.SYNOPSIS
    Quản lý version cho NuGet package

.DESCRIPTION
    Script PowerShell để bump version theo Semantic Versioning (SemVer)
    Hỗ trợ: major, minor, patch, alpha, beta, rc, release

.PARAMETER Type
    Loại version bump: major, minor, patch, alpha, beta, rc, release

.PARAMETER ProjectFile
    Đường dẫn tới file .csproj (mặc định: src/FSRS.Core/FSRS.Core.csproj)

.EXAMPLE
    .\Bump-Version.ps1 -Type patch
    .\Bump-Version.ps1 -Type minor
    .\Bump-Version.ps1 -Type alpha

.NOTES
    Tác giả: TranPhucTien
    Ngày tạo: 2025-05-26
#>

param(
    [Parameter(Mandatory=$true, HelpMessage="Chọn loại version bump")]
    [ValidateSet("major", "minor", "patch", "alpha", "beta", "rc", "release")]
    [string]$Type,
    
    [Parameter(Mandatory=$false)]
    [string]$ProjectFile = "src\FSRS.Core\FSRS.Core.csproj",
    
    [Parameter(Mandatory=$false)]
    [switch]$Force,
    
    [Parameter(Mandatory=$false)]
    [switch]$NoChangelog
)

# 🎨 Colors for output
$Colors = @{
    Red = "Red"
    Green = "Green"
    Yellow = "Yellow"
    Blue = "Cyan"
    Purple = "Magenta"
    White = "White"
    Gray = "Gray"
}

# 📝 Function để viết output có màu
function Write-ColoredOutput {
    param(
        [string]$Message,
        [string]$Color = "White",
        [switch]$NoNewline
    )
    
    if ($NoNewline) {
        Write-Host $Message -ForegroundColor $Color -NoNewline
    } else {
        Write-Host $Message -ForegroundColor $Color
    }
}

# 🎯 Function hiển thị banner
function Show-Banner {
    Write-ColoredOutput "╔════════════════════════════════════════╗" $Colors.Blue
    Write-ColoredOutput "║          🚀 Version Manager            ║" $Colors.Blue
    Write-ColoredOutput "║       Semantic Versioning Tool        ║" $Colors.Blue
    Write-ColoredOutput "╚════════════════════════════════════════╝" $Colors.Blue
    Write-ColoredOutput ""
}

# 🔍 Function kiểm tra dependencies
function Test-Dependencies {
    Write-ColoredOutput "🔍 Checking dependencies..." $Colors.Blue
    
    # Kiểm tra Git
    try {
        $null = Get-Command git -ErrorAction Stop
        Write-ColoredOutput "✅ Git: Found" $Colors.Green
    } catch {
        Write-ColoredOutput "❌ Git: Not found" $Colors.Red
        Write-ColoredOutput "💡 Please install Git: https://git-scm.com/download/win" $Colors.Yellow
        exit 1
    }
    
    # Kiểm tra đang trong Git repository
    try {
        $null = git rev-parse --git-dir 2>$null
        Write-ColoredOutput "✅ Git Repository: Valid" $Colors.Green
    } catch {
        Write-ColoredOutput "❌ Not in a Git repository" $Colors.Red
        exit 1
    }
    
    # Kiểm tra uncommitted changes
    $gitStatus = git status --porcelain
    if ($gitStatus) {
        Write-ColoredOutput "⚠️  Warning: You have uncommitted changes" $Colors.Yellow
        Write-ColoredOutput $gitStatus $Colors.Gray
        
        if (-not $Force) {
            $response = Read-Host "Continue anyway? (y/N)"
            if ($response -ne "y" -and $response -ne "Y") {
                Write-ColoredOutput "⏹️  Operation cancelled" $Colors.Yellow
                exit 0
            }
        }
    }
    
    Write-ColoredOutput "✅ Dependencies check passed" $Colors.Green
    Write-ColoredOutput ""
}

# 📋 Function hiển thị trạng thái hiện tại
function Show-CurrentStatus {
    Write-ColoredOutput "📋 Current Status:" $Colors.Blue
    
    $currentDir = Split-Path -Leaf (Get-Location)
    $currentBranch = git branch --show-current
    $lastCommit = git log -1 --pretty=format:"%h - %s (%cr)" 2>$null
    
    Write-ColoredOutput "  📂 Project: " $Colors.Gray -NoNewline
    Write-ColoredOutput $currentDir $Colors.White
    
    Write-ColoredOutput "  🌿 Branch: " $Colors.Gray -NoNewline
    Write-ColoredOutput $currentBranch $Colors.White
    
    Write-ColoredOutput "  📝 Last commit: " $Colors.Gray -NoNewline
    Write-ColoredOutput $lastCommit $Colors.White
    
    Write-ColoredOutput "  📦 Project file: " $Colors.Gray -NoNewline
    Write-ColoredOutput $ProjectFile $Colors.White
    
    Write-ColoredOutput ""
}

# 🔍 Function kiểm tra project file
function Test-ProjectFile {
    if (-not (Test-Path $ProjectFile)) {
        Write-ColoredOutput "❌ Project file not found: $ProjectFile" $Colors.Red
        
        Write-ColoredOutput "💡 Available .csproj files:" $Colors.Yellow
        Get-ChildItem -Path . -Filter "*.csproj" -Recurse | Select-Object -First 5 | ForEach-Object {
            Write-ColoredOutput "   $($_.FullName)" $Colors.Gray
        }
        
        exit 1
    }
    
    Write-ColoredOutput "✅ Project file found" $Colors.Green
}

# 📖 Function đọc version hiện tại
function Get-CurrentVersion {
    $projectContent = Get-Content $ProjectFile -Raw
    
    # Tìm Version tag
    $versionMatch = [regex]::Match($projectContent, '<Version>([^<]+)</Version>')
    
    if ($versionMatch.Success) {
        $script:CurrentVersion = $versionMatch.Groups[1].Value
        Write-ColoredOutput "📦 Current version: " $Colors.Blue -NoNewline
        Write-ColoredOutput $script:CurrentVersion $Colors.Green
        return
    }
    
    # Thử tìm PackageVersion
    $packageVersionMatch = [regex]::Match($projectContent, '<PackageVersion>([^<]+)</PackageVersion>')
    if ($packageVersionMatch.Success) {
        $script:CurrentVersion = $packageVersionMatch.Groups[1].Value
        Write-ColoredOutput "📦 Found PackageVersion: " $Colors.Blue -NoNewline
        Write-ColoredOutput $script:CurrentVersion $Colors.Green
        return
    }
    
    # Thử tìm VersionPrefix
    $versionPrefixMatch = [regex]::Match($projectContent, '<VersionPrefix>([^<]+)</VersionPrefix>')
    if ($versionPrefixMatch.Success) {
        $script:CurrentVersion = $versionPrefixMatch.Groups[1].Value
        Write-ColoredOutput "📦 Found VersionPrefix: " $Colors.Blue -NoNewline
        Write-ColoredOutput $script:CurrentVersion $Colors.Green
        return
    }
    
    Write-ColoredOutput "❌ No version found in project file" $Colors.Red
    Write-ColoredOutput "💡 Make sure your .csproj has <Version>1.0.0</Version> tag" $Colors.Yellow
    exit 1
}

# 🧮 Function phân tích version
function Parse-Version {
    # Kiểm tra pre-release
    $script:IsPrerelease = $script:CurrentVersion.Contains("-")
    
    if ($script:IsPrerelease) {
        $parts = $script:CurrentVersion.Split("-")
        $script:BaseVersion = $parts[0]
        $script:PrereleaseTag = $parts[1]
        Write-ColoredOutput "⚠️  Pre-release version detected: " $Colors.Yellow -NoNewline
        Write-ColoredOutput $script:PrereleaseTag $Colors.Purple
    } else {
        $script:BaseVersion = $script:CurrentVersion
        $script:PrereleaseTag = ""
    }
    
    # Parse version numbers
    $versionParts = $script:BaseVersion.Split(".")
    
    if ($versionParts.Count -ne 3) {
        Write-ColoredOutput "❌ Invalid version format. Expected: MAJOR.MINOR.PATCH" $Colors.Red
        Write-ColoredOutput "Current: $script:CurrentVersion" $Colors.Gray
        exit 1
    }
    
    try {
        $script:Major = [int]$versionParts[0]
        $script:Minor = [int]$versionParts[1]
        $script:Patch = [int]$versionParts[2]
    } catch {
        Write-ColoredOutput "❌ Invalid version numbers" $Colors.Red
        exit 1
    }
    
    Write-ColoredOutput "🔢 Version parts: " $Colors.Blue -NoNewline
    Write-ColoredOutput "Major=$script:Major, Minor=$script:Minor, Patch=$script:Patch" $Colors.White
}

# 🎯 Function tính toán version mới
function Calculate-NewVersion {
    param([string]$BumpType)
    
    switch ($BumpType) {
        "major" {
            $script:Major++
            $script:Minor = 0
            $script:Patch = 0
            $script:NewVersion = "$script:Major.$script:Minor.$script:Patch"
            Write-ColoredOutput "💥 MAJOR version bump (Breaking changes)" $Colors.Red
        }
        "minor" {
            $script:Minor++
            $script:Patch = 0
            $script:NewVersion = "$script:Major.$script:Minor.$script:Patch"
            Write-ColoredOutput "✨ MINOR version bump (New features)" $Colors.Yellow
        }
        "patch" {
            $script:Patch++
            $script:NewVersion = "$script:Major.$script:Minor.$script:Patch"
            Write-ColoredOutput "🐛 PATCH version bump (Bug fixes)" $Colors.Green
        }
        "alpha" {
            if ($script:CurrentVersion -match "alpha\.(\d+)") {
                $alphaNum = [int]$matches[1] + 1
                $script:NewVersion = "$script:Major.$script:Minor.$script:Patch-alpha.$alphaNum"
            } else {
                $script:NewVersion = "$script:Major.$script:Minor.$script:Patch-alpha.1"
            }
            Write-ColoredOutput "🔬 ALPHA pre-release" $Colors.Blue
        }
        "beta" {
            if ($script:CurrentVersion -match "beta\.(\d+)") {
                $betaNum = [int]$matches[1] + 1
                $script:NewVersion = "$script:Major.$script:Minor.$script:Patch-beta.$betaNum"
            } else {
                $script:NewVersion = "$script:Major.$script:Minor.$script:Patch-beta.1"
            }
            Write-ColoredOutput "🧪 BETA pre-release" $Colors.Blue
        }
        "rc" {
            if ($script:CurrentVersion -match "rc\.(\d+)") {
                $rcNum = [int]$matches[1] + 1
                $script:NewVersion = "$script:Major.$script:Minor.$script:Patch-rc.$rcNum"
            } else {
                $script:NewVersion = "$script:Major.$script:Minor.$script:Patch-rc.1"
            }
            Write-ColoredOutput "🚀 RELEASE CANDIDATE" $Colors.Blue
        }
        "release" {
            if ($script:IsPrerelease) {
                $script:NewVersion = "$script:Major.$script:Minor.$script:Patch"
                Write-ColoredOutput "🎉 Converting pre-release to STABLE release" $Colors.Green
            } else {
                Write-ColoredOutput "❌ Current version is already a stable release" $Colors.Red
                exit 1
            }
        }
    }
    
    Write-ColoredOutput ""
    Write-ColoredOutput "🎯 New version: " $Colors.Green -NoNewline
    Write-ColoredOutput $script:NewVersion $Colors.Yellow
}

# ✏️ Function cập nhật version trong file
function Update-VersionInFile {
    param([string]$NewVersion)
    
    Write-ColoredOutput "🔄 Updating version in project file..." $Colors.Yellow
    
    # Tạo backup
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupFile = "$ProjectFile.backup.$timestamp"
    Copy-Item $ProjectFile $backupFile
    Write-ColoredOutput "💾 Backup created: $backupFile" $Colors.Blue
    
    # Đọc nội dung file
    $projectContent = Get-Content $ProjectFile -Raw
    
    # Update Version tag
    $projectContent = $projectContent -replace '<Version>[^<]+</Version>', "<Version>$NewVersion</Version>"
    
    # Update AssemblyVersion nếu có
    if ($projectContent -match '<AssemblyVersion>') {
        $projectContent = $projectContent -replace '<AssemblyVersion>[^<]+</AssemblyVersion>', "<AssemblyVersion>$NewVersion.0</AssemblyVersion>"
    }
    
    # Update FileVersion nếu có
    if ($projectContent -match '<FileVersion>') {
        $projectContent = $projectContent -replace '<FileVersion>[^<]+</FileVersion>', "<FileVersion>$NewVersion.0</FileVersion>"
    }
    
    # Ghi lại file
    Set-Content $ProjectFile $projectContent -Encoding UTF8
    
    Write-ColoredOutput "✅ Version updated successfully!" $Colors.Green
    
    # Hiển thị thay đổi
    Write-ColoredOutput ""
    Write-ColoredOutput "📋 Changes made:" $Colors.Blue
    Write-ColoredOutput "Before: " $Colors.Gray -NoNewline
    Write-ColoredOutput $script:CurrentVersion $Colors.Yellow
    Write-ColoredOutput "After:  " $Colors.Gray -NoNewline
    Write-ColoredOutput $NewVersion $Colors.Green
}

# 📝 Function tạo changelog entry
function New-ChangelogEntry {
    param(
        [string]$Version,
        [string]$BumpType
    )
    
    if ($NoChangelog) {
        Write-ColoredOutput "⏭️  Skipping changelog creation" $Colors.Gray
        return
    }
    
    Write-ColoredOutput "📝 Creating changelog entry..." $Colors.Blue
    
    $changelogFile = "CHANGELOG.md"
    $currentDate = Get-Date -Format "yyyy-MM-dd"
    
    # Tạo CHANGELOG.md nếu chưa có
    if (-not (Test-Path $changelogFile)) {
        Write-ColoredOutput "📄 Creating new CHANGELOG.md" $Colors.Yellow
        
        $changelogHeader = @"
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

"@
        Set-Content $changelogFile $changelogHeader -Encoding UTF8
    }
    
    # Đọc changelog hiện tại
    $existingContent = Get-Content $changelogFile -Raw
    
    # Tạo entry mới
    $newEntry = switch ($BumpType) {
        "major" {
            @"

## [$Version] - $currentDate

### 💥 BREAKING CHANGES
- 

### ✨ New Features
- 

### 🐛 Bug Fixes
- 

"@
        }
        "minor" {
            @"

## [$Version] - $currentDate

### ✨ New Features
- 

### 🐛 Bug Fixes
- 

### 🔧 Improvements
- 

"@
        }
        "patch" {
            @"

## [$Version] - $currentDate

### 🐛 Bug Fixes
- 

### 🔧 Improvements
- 

"@
        }
        default {
            @"

## [$Version] - $currentDate

### 🚀 Changes
- 

"@
        }
    }
    
    # Chèn entry mới vào đầu changelog (sau header)
    $lines = $existingContent -split "`r?`n"
    $headerEndIndex = 5  # Dòng kết thúc của header
    
    $newContent = ($lines[0..$headerEndIndex] -join "`n") + $newEntry + "`n" + ($lines[($headerEndIndex + 1)..($lines.Length - 1)] -join "`n")
    
    Set-Content $changelogFile $newContent -Encoding UTF8
    Write-ColoredOutput "✅ Changelog entry created" $Colors.Green
}

# 🎯 Function hiển thị next steps
function Show-NextSteps {
    param([string]$Version)
    
    Write-ColoredOutput ""
    Write-ColoredOutput "🎉 Version bump completed!" $Colors.Green
    Write-ColoredOutput ""
    Write-ColoredOutput "📋 Next steps:" $Colors.Blue
    Write-ColoredOutput "  1. Review changes:" $Colors.Gray
    Write-ColoredOutput "     git diff" $Colors.Yellow
    Write-ColoredOutput ""
    Write-ColoredOutput "  2. Commit changes:" $Colors.Gray
    Write-ColoredOutput "     git add ." $Colors.Yellow
    Write-ColoredOutput "     git commit -m `"Bump version to $Version`"" $Colors.Yellow
    Write-ColoredOutput ""
    Write-ColoredOutput "  3. Create and push tag:" $Colors.Gray
    Write-ColoredOutput "     git tag -a `"v$Version`" -m `"Release version $Version`"" $Colors.Yellow
    Write-ColoredOutput "     git push origin main" $Colors.Yellow
    Write-ColoredOutput "     git push origin `"v$Version`"" $Colors.Yellow
    Write-ColoredOutput ""
    Write-ColoredOutput "  4. GitHub Actions will automatically build and publish! 🚀" $Colors.Green
}

# 🚀 MAIN EXECUTION
try {
    Show-Banner
    Show-CurrentStatus
    Test-Dependencies
    Test-ProjectFile
    Get-CurrentVersion
    Parse-Version
    Calculate-NewVersion $Type
    
    Write-ColoredOutput ""
    if (-not $Force) {
        $confirmation = Read-Host "Continue with version bump? (y/N)"
        if ($confirmation -ne "y" -and $confirmation -ne "Y") {
            Write-ColoredOutput "⏹️  Operation cancelled" $Colors.Yellow
            exit 0
        }
    }
    
    Update-VersionInFile $script:NewVersion
    
    # Tạo changelog entry chỉ cho stable releases
    if ($script:NewVersion -notmatch "-") {
        New-ChangelogEntry $script:NewVersion $Type
    }
    
    Show-NextSteps $script:NewVersion
    
} catch {
    Write-ColoredOutput "❌ Error occurred: $($_.Exception.Message)" $Colors.Red
    Write-ColoredOutput "Stack trace: $($_.ScriptStackTrace)" $Colors.Gray
    exit 1
}