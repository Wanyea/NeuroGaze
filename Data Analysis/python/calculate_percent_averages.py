from zipfile import ZipFile
import os
import csv

# Define the paths
zip_file_path = "C:/source/NeuroGaze/unity/Assets/Assessment_Results/Assessment_Results.zip"
extract_to_folder = "C:/source/NeuroGaze/unity/Assets/Assessment_Results/"

# Extract the ZIP file
with ZipFile(zip_file_path, 'r') as zip_ref:
    zip_ref.extractall(extract_to_folder)

# List the top-level directories after extraction
extracted_folders = next(os.walk(extract_to_folder))[1]
extracted_folders_full_paths = [os.path.join(extract_to_folder, folder) for folder in extracted_folders]

# Placeholder for results
results = []

# Process each CSV file for every user
for folder_path in extracted_folders_full_paths:
    folder_name = os.path.basename(folder_path)
    for filename in os.listdir(folder_path):
        if filename.endswith(".csv"):
            user_number = filename.split("_")[-1].split(".")[0]
            print(user_number)
            file_path = os.path.join(folder_path, filename)
            with open(file_path, mode='r', encoding='utf-8') as file:
                csv_reader = csv.reader(file)
                total_error_count, total_interactables, total_round_duration = 0, 0, 0
                trial_count = 0
                next(csv_reader)  # Skip the header row
                for row in csv_reader:
                    # Adjusting indices to 0-based and ensuring they match your CSV structure
                    total_error_count += int(row[2])  # Assuming 'Error Count' is the 3rd column
                    total_interactables += int(row[3])  # Assuming 'Total Eye Interactables' is the 4th column
                    total_round_duration += float(row[1])  # Assuming 'Round Duration' is the 2nd column
                    trial_count += 1
                # Calculate averages
                avg_round_duration = total_round_duration / trial_count
                gross_error_rate = total_error_count / (total_error_count + total_interactables) if (total_error_count + total_interactables) > 0 else 0
                results.append([f"{filename}_{user_number}", folder_name, avg_round_duration, gross_error_rate])

# Write results to a new CSV file
output_file_path = os.path.join(extract_to_folder, "analysis_results.csv")
with open(output_file_path, mode='w', newline='', encoding='utf-8') as file:
    csv_writer = csv.writer(file)
    csv_writer.writerow(["FileName_UserNumber", "FolderName", "Average Round Duration", "Gross Error Rate"])
    csv_writer.writerows(results)

print(f"Analysis completed. Results are stored in {output_file_path}")
