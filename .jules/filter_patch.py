
import re
import sys

def filter_patch(input_patch_path, output_patch_path):
    # Patterns for files to exclude
    exclude_patterns = [
        r'bin/',
        r'obj/',
        r'\.csproj\.nuget\.dgspec\.json',
        r'\.csproj\.nuget\.g\.props',
        r'project\.assets\.json',
        r'project\.nuget\.cache',
        r'AssemblyInfo\.cs',
        r'AssemblyInfoInputs\.cache',
        r'GeneratedMSBuildEditorConfig\.editorconfig',
        r'csproj\.CoreCompileInputs\.cache',
        r'csproj\.FileListAbsolute\.txt',
        r'csproj\.AssemblyReference\.cache',
        r'sourcelink\.json',
        r'Up2Date',
        r'genruntimeconfig\.cache',
        r'staticwebassets\.build\.endpoints\.json',
        r'staticwebassets\.build\.json',
        r'staticwebassets\.development\.json',
        r'staticwebassets/msbuild\.HarborFlowSuite\.Server\.Microsoft\.AspNetCore\.StaticWebAssets\.props',
        r'staticwebassets/msbuild\.HarborFlowSuite\.Server\.Microsoft\.AspNetCore\.StaticWebAssetEndpoints\.props',
        r'staticwebassets/msbuild\.build\.HarborFlowSuite\.Server\.props',
        r'staticwebassets/msbuild\.buildMultiTargeting\.HarborFlowSuite\.Server\.props',
        r'staticwebassets/msbuild\.buildTransitive\.HarborFlowSuite\.Server\.props',
        r'staticwebassets\.pack\.json',
        r'HarborFlowSuite\.Server\.MvcApplicationPartsAssemblyInfo\.cs',
        r'HarborFlowSuite\.Server\.MvcApplicationPartsAssemblyInfo\.cache',
        r'HarborFlowSuite\.Application\.dll', # Binary files
        r'HarborFlowSuite\.Application\.pdb', # Binary files
        r'HarborFlowSuite\.Core\.dll', # Binary files
        r'HarborFlowSuite\.Core\.pdb', # Binary files
        r'HarborFlowSuite\.Infrastructure\.dll', # Binary files
        r'HarborFlowSuite\.Infrastructure\.pdb', # Binary files
        r'HarborFlowSuite\.Server\.dll', # Binary files
        r'HarborFlowSuite\.Server\.pdb', # Binary files
        r'apphost', # Binary file
    ]

    # Compile regex patterns for efficiency
    compiled_exclude_patterns = [re.compile(p) for p in exclude_patterns]

    with open(input_patch_path, 'r') as infile, open(output_patch_path, 'w') as outfile:
        skip_current_file = False
        for line in infile:
            if line.startswith('diff --git'):
                # Extract the file path from the diff header
                # Example: diff --git a/path/to/file b/path/to/file
                file_path_match = re.search(r'b/(.+)', line)
                if file_path_match:
                    file_path = file_path_match.group(1)
                    skip_current_file = False
                    for pattern in compiled_exclude_patterns:
                        if pattern.search(file_path):
                            skip_current_file = True
                            print(f"Skipping changes for: {file_path}")
                            break
                else:
                    skip_current_file = False # Should not happen for valid diffs

            if not skip_current_file:
                outfile.write(line)

if __name__ == "__main__":
    input_patch = ".jules/diff.patch"
    output_patch = ".jules/filtered.patch"
    filter_patch(input_patch, output_patch)
