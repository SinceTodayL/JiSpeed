# --- Configuration ---
$repoPath = "E:\_2024-2025_Undergraduate_Year2_Spring\DataBase_Design\frontend-dist"
$maxSize = 104857600 # 100MB in bytes

# --- Script Body ---
try {
    # Change to the repository directory
    Set-Location -Path $repoPath

    Write-Host "[+] Staging all changes..." -ForegroundColor Cyan
    git add .

    Write-Host "[+] Checking for oversized files..." -ForegroundColor Cyan
    
    # Get a list of staged files
    $stagedFiles = git diff --staged --name-only
    
    # Find all files that are too large
    $largeFiles = @()
    if ($stagedFiles) {
        foreach ($file in $stagedFiles) {
            # Check if file exists before getting its size (e.g., for deleted files)
            if (Test-Path -LiteralPath $file) {
                $fileSize = (Get-Item -LiteralPath $file).Length
                if ($fileSize -gt $maxSize) {
                    # Add the oversized file object to our list
                    $largeFiles += [pscustomobject]@{
                        Path = $file
                        Size = $fileSize
                    }
                }
            }
        }
    }

    # --- Commit and Push Logic ---
    if ($largeFiles.Count -gt 0) {
        Write-Host ""
        Write-Host "[!] ERROR: Oversized files detected. Commit aborted." -ForegroundColor Red
        Write-Host "[!] The following files exceed the size limit:" -ForegroundColor Yellow
        foreach ($file in $largeFiles) {
            Write-Host ("      - {0} (Size: {1} bytes)" -f $file.Path, $file.Size) -ForegroundColor Yellow
        }
        Write-Host "[!] Please remove them from the commit or use Git LFS." -ForegroundColor Yellow
    } elseif ($stagedFiles) {
        Write-Host "[+] All checks passed. Committing and pushing..." -ForegroundColor Green
        
        $timestamp = Get-Date -Format "yyyyMMdd"
        $commitMessage = "update at $timestamp"
        
        git commit -m $commitMessage
        git push origin main
        
        Write-Host "[*] Successfully committed and pushed to main branch (Timestamp: $timestamp)." -ForegroundColor Green
    } else {
        Write-Host "[*] No content to commit." -ForegroundColor White
    }

} catch {
    # This block will catch any unexpected errors during script execution
    Write-Host ""
    Write-Host "An unexpected error occurred:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
} finally {
    Write-Host ""
    Read-Host "Script finished. Press Enter to exit..."
}