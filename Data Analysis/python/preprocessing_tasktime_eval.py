import pandas as pd

# Reading the original CSV file into a DataFrame
df = pd.read_csv('not_averaged_analysis_results.csv')

# Initialize a new DataFrame with the desired structure
# Assuming FolderName values include 'Eye_Gaze_Hands', 'Neuro_Gaze', 'VR_Controllers'
structured_df_columns = ['Eye_Gaze_Hands', 'Neuro_Gaze', 'VR_Controllers']
structured_df = pd.DataFrame(columns=structured_df_columns)

# Since we don't care about the participant number, we use a single row approach
structured_data = {col: [] for col in structured_df_columns}  # Preparing a dictionary to hold our data

for _, row in df.iterrows():
    folder_name = row['FolderName']
    gross_error_rate = row['Average Round Duration']
    if folder_name in structured_data:
        structured_data[folder_name].append(gross_error_rate)

# Converting lists in structured_data to single values for each column, assuming a single entry per column is needed
# If multiple entries per column are possible, this part needs adjustment
for key, values in structured_data.items():
    structured_df[key] = pd.Series(values)

# Saving the structured DataFrame to a new CSV file
output_csv_path = 'round_duration.csv'
structured_df.to_csv(output_csv_path, index=False)

print(f"Structured CSV saved to: {output_csv_path}")
