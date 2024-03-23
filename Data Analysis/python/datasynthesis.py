from zipfile import ZipFile
import os

# Path to the uploaded ZIP file
zip_file_path = "C:/source/NeuroGaze/unity/Assets/Assessment_Results/Assessment_Results.zip"
extract_to_folder = "C:/source/NeuroGaze/unity/Assets/Assessment_Results/"

# Extract the ZIP file
with ZipFile(zip_file_path, 'r') as zip_ref:
    zip_ref.extractall(extract_to_folder)

# Verify extraction and list the top-level directories
extracted_folders = next(os.walk(extract_to_folder))[1]
extracted_folders_full_paths = [os.path.join(extract_to_folder, folder) for folder in extracted_folders]

print(extracted_folders_full_paths)


# Dictionary to hold user numbers and their files across folders
user_files = {}

# Function to update the user_files dictionary with filenames
def update_user_files(folder):
    for filename in os.listdir(folder):
        if filename.endswith(".csv"):
            # Extract user number from filename
            user_number = filename.split("_")[-1].split(".")[0]
            if user_number not in user_files:
                user_files[user_number] = []
            user_files[user_number].append(os.path.join(folder, filename))

# Update user_files with data from each folder
for folder_path in extracted_folders_full_paths:
    update_user_files(folder_path)

# Check for users with incomplete file sets and print results
complete_users = []
incomplete_users_files = {}

for user_number, files in user_files.items():
    if len(files) == 3:  # A complete set of files for a user
        complete_users.append(user_number)
    else:
        incomplete_users_files[user_number] = files

# complete_users, incomplete_users_files
print(incomplete_users_files)