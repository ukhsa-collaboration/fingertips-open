

# Lines that will be matched
$regexElements = @(
    "^  \w+\(", # some file indent with 2 spaces
    "^    \w+\(",
    "^    public \w+\(",
    "^    static \w+\(",
    "^    public static \w+\(",
    "^export class"
)

$regex = [string]::Join('|', $regexElements)

function WriteDocs($filePath) {

    # Write file path
    
    $shortPath = $filePath -replace ".*app\\", ""
    $line = "-" * $shortPath.Length
    write "", $shortPath, $line

    # List classes and methods
    gc $filePath | 
        Select-String -Pattern $regex | 
        Select-String -Pattern "constructor" -NotMatch | 
        % { $_ -replace "export class ", "" }| 
        % { $_ -replace "{.*", "" } | 
        % { $_ -replace "public ", "" }| 
        % { $_ -replace "static ", "" }| 
        % { $_ -replace "\(.*", "" }
}

function WriteDocsInDir($dir) {

    $files = Get-ChildItem $dir -file | ? { $_ -notmatch ".spec." -and $_ -notmatch ".module."}

    foreach ($file in $files) {
        WriteDocs $file.FullName
    }
}

# Write docs
$root = 'src/app'
WriteDocsInDir "$root/shared"
WriteDocsInDir "$root/shared/service/helper"
