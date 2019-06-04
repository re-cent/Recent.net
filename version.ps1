if($Env:Build.SourceBranchName -eq "develop"){
	$PreRelease = $Env:Version+"-dev"
	Write-Host "##vso[task.setvariable variable=Version]$(PreRelease)"
}
if($Env:Build.SourceBranchName -eq "merge"){
	$PreRelease = $Env:Version+"-beta"
	Write-Host "##vso[task.setvariable variable=Version]$(PreRelease)"
}
