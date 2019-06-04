if($Env:Build.SourceBranchName -eq "develop"){
	$PreRelease = $Env:Version+"-dev"
	echo "##vso[task.setvariable variable=Version]$(PreRelease)"
}
if($Env:Build.SourceBranchName -eq "merge"){
	$PreRelease = $Env:Version+"-beta"
	echo "##vso[task.setvariable variable=Version]$(PreRelease)"
}
