import numpy as np
import pandas as pd

# Since there was an issue with directly displaying the file content, let's proceed with the task based on the given instructions.
not_averaged_data_path = 'not_averaged_analysis_results.csv'
# First, re-reading the uploaded file to ensure access to its data.
not_averaged_data = pd.read_csv(not_averaged_data_path)

# Extracting unique FolderNames for reference
folder_names = not_averaged_data['FolderName'].unique()

# Calculating mean and standard deviation for Average Round Duration and Gross Error Rate by FolderName
stats_by_folder = not_averaged_data.groupby('FolderName').agg({
    'Average Round Duration': ['mean', 'std'],
    'Gross Error Rate': ['mean', 'std']
}).reset_index()

# Preparing new data for users 17-20
new_users_data = []

for user_id in range(17, 21):
    for folder_name in folder_names:
        # Extracting statistics for the current folder
        stats = stats_by_folder[stats_by_folder['FolderName'] == folder_name]
        
        # Generating random Average Round Duration and Gross Error Rate based on mean and std
        avg_duration = np.random.normal(
            loc=stats['Average Round Duration']['mean'].values[0],
            scale=stats['Average Round Duration']['std'].values[0] / 2)
        gross_error = np.random.normal(
            loc=stats['Gross Error Rate']['mean'].values[0],
            scale=stats['Gross Error Rate']['std'].values[0] / 2)
        
        # Ensuring the generated values are not negative
        avg_duration = max(avg_duration, 0)
        gross_error = max(round(gross_error), 0)
        
        # Creating the new user entry
        new_users_data.append({
            'FileName_UserNumber': f'Static{folder_name}Results_{user_id}.csv_{user_id}',
            'FolderName': folder_name,
            'Average Round Duration': avg_duration,
            'Gross Error Rate': gross_error
        })

# Creating a DataFrame for the new user data
new_users_df = pd.DataFrame(new_users_data)

# Appending the new users data to the existing DataFrame
updated_not_averaged_data = pd.concat([not_averaged_data, new_users_df], ignore_index=True)

# Saving the updated DataFrame to a new CSV file
updated_not_averaged_file_path = 'round_duration.csv'
updated_not_averaged_data.to_csv(updated_not_averaged_file_path, index=False)

updated_not_averaged_file_path
