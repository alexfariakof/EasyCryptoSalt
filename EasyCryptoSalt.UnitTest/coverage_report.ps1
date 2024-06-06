cls

$projectTestPath = Get-Location
$projectPath =  (Resolve-Path -Path ..).Path
$sourceDirs = "$projectPath\EasyCryptoSalt;"
$reportPath = Join-Path -Path $projectTestPath -ChildPath "TestResults"
$coverageXmlPath = Join-Path -Path (Join-Path -Path $projectTestPath -ChildPath "TestResults") -ChildPath "coveragereport"

 function Wait-TestResults {
    $REPEAT_WHILE = 0
    while (-not (Test-Path $reportPath)) {
        echo "Agaurdando TestResults..."
        Start-Sleep -Seconds 10        
        if ($REPEAT_WHILE -eq 6) { break }
        $REPEAT_WHILE = $REPEAT_WHILE + 1
    }

    $REPEAT_WHILE = 0
    while (-not (Test-Path $coverageXmlPath)) {
        echo "Agaurdando Coverage Report..."
        Start-Sleep -Seconds 10        
        if ($REPEAT_WHILE -eq 6) { break }
        $REPEAT_WHILE = $REPEAT_WHILE + 1
    }   

 } 

dotnet test ./EasyCryptoSalt.UnitTest.csproj --results-directory $reportPath /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura --collect:"XPlat Code Coverage;Format=opencover" --no-restore --no-build > $null 2>&1
reportgenerator -reports:$projectTestPath\coverage.cobertura.xml -targetdir:$coverageXmlPath -reporttypes:"Html;lcov;" -sourcedirs:$sourceDirs > $null 2>&1
Wait-TestResults
Invoke-Item $coverageXmlPath\index.html