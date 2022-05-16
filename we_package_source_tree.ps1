#
#	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
#	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
#

cls

$this_dir_path = [System.Environment]::CurrentDirectory


$src_dir_name = "src"
$src_dir_path = "$this_dir_path\$src_dir_name"

echo "SRC_DIR_PATH: $src_dir_path"


$nupkg_files = $null # can be explicitly set here or $null for auto-discovery

if ($nupkg_files -eq $null)
{
	$nupkg_files = Get-ChildItem -Recurse "$src_dir_path\*.nupkg" | Select-Object -Property Name, Directory, FullName

	if ($nupkg_files -eq $null)
	{ echo "An error occurred during the operation (NuGet Package file discovery)."; return; }

	foreach ($nupkg_file in $nupkg_files)
	{
		echo ("NUPKG_FILE[]: " + $nupkg_file)
	}
}


$nuget_local_cache_dir_path = "D:\development\wellengineered-us\_nuget_local_"

$nuget_dir_path = "C:\Program Files\dotnet"
$nuget_file_name = "dotnet.exe"
$nuget_command = "nuget"
$nuget_exe = "$nuget_dir_path\$nuget_file_name"

echo "NUGET_DIR_PATH: $nuget_dir_path"
echo "NUGET_FILE_NAME: $nuget_file_name"
echo "NUGET_COMMAND: $nuget_command"
echo "NUGET_EXE: $nuget_exe"


echo "The operation is starting..."

if (!(Test-Path -Path $nuget_local_cache_dir_path))
{ echo "An error occurred during the operation (NuGet local cache dir path: '$nuget_local_cache_dir_path')."; return; }

foreach ($nupkg_file in $nupkg_files)
{
	$nupkg_file_path = $nupkg_file.FullName

	echo "NUPKG_FILE_PATH: $nupkg_file_path"

	&"$nuget_exe" $nuget_command push "$nupkg_file_path" -s "$nuget_local_cache_dir_path"

	if (!($LastExitCode -eq $null -or $LastExitCode -eq 0))
	{ echo "An error occurred during the operation."; return; }
}

echo "The operation completed successfully."
