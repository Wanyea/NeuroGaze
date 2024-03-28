import os
import csv

# Define the path to the folder containing subfolders with CSV files
root_folder_path = "C:/source/NeuroGaze/unity/Assets/Assessment_Results/Assessment_Results/"

# Placeholder for results
results = []

# Process each CSV file for every user
for root, dirs, files in os.walk(root_folder_path):
    for filename in files:
        if filename.endswith(".csv"):
            user_number = filename.split("_")[-1].split(".")[0]
            print(user_number)
            file_path = os.path.join(root, filename)
            with open(file_path, mode='r', encoding='utf-8') as file:
                csv_reader = csv.reader(file)
                total_error_count, total_interactables, total_round_duration = 0, 0, 0
                trial_count = 0
                next(csv_reader)  # Skip the header row
                for row in csv_reader:
                    # Adjusting indices to 0-based and ensuring they match your CSV structure
                    error_count = int(row[2])  # Assuming 'Error Count' is the 3rd column
                    interactables = int(row[3])  # Assuming 'Total Eye Interactables' is the 4th column
                    round_duration = float(row[1])  # Assuming 'Round Duration' is the 2nd column
                    trial_count += 1
                
                    folder_name = os.path.basename(os.path.dirname(file_path))
                    results.append([f"{filename}_{user_number}", folder_name, round_duration, error_count])

# Write results to a new CSV file
output_file_path = os.path.join(root_folder_path, "_not_averaged_analysis_results.csv")
with open(output_file_path, mode='w', newline='', encoding='utf-8') as file:
    csv_writer = csv.writer(file)
    csv_writer.writerow(["FileName_UserNumber", "FolderName", "Average Round Duration", "Gross Error Rate"])
    csv_writer.writerows(results)

print(f"Analysis completed. Results are stored in {output_file_path}")
