import pandas as pd
from scipy.stats import friedmanchisquare

def run_friedman_test(csv_path, output_csv_path):
    # Load the CSV file
    df = pd.read_csv(csv_path)
    
    # Map techniques to numeric codes
    technique_mapping = {
        'NeuroGaze': 1,
        'Eye Tracking + Hand Tracking': 2,
        'VR Controllers': 3
    }
    df['Technique Code'] = df['Q2'].map(technique_mapping)

    # Identify the questions to analyze
    questions = [col for col in df.columns if 'Q3' in col]

    # Prepare the results dictionary
    friedman_results = {}

    for question in questions:
        data_for_analysis = []
        for participant in df['Q1'].unique():
            participant_data = df[df['Q1'] == participant]
            if participant_data.shape[0] == 3:  # Ensure all three techniques are evaluated
                data_for_analysis.append(participant_data[question].tolist())

        data_for_analysis_transposed = list(map(list, zip(*data_for_analysis)))

        if len(data_for_analysis_transposed) == 3 and len(data_for_analysis_transposed[0]) > 2:
            stat, p = friedmanchisquare(*data_for_analysis_transposed)
            friedman_results[question] = {'Statistic': stat, 'P-Value': p}

    # Convert the results dictionary to a DataFrame for easy CSV export
    results_df = pd.DataFrame(friedman_results).transpose()
    results_df.to_csv(output_csv_path, index_label='Question')

# Specify your CSV path and the desired output path for the results
csv_path = 'D:/source/NeuroGaze/unity/Assets/Assessment_Results/Complete_NASA_TLX/Complete_NASA_TLX.csv'
output_csv_path = 'D:/source/NeuroGaze/unity/Assets/Assessment_Results/Complete_NASA_TLX/NASA_TLX_Analysis.csv'

# Run the analysis
run_friedman_test(csv_path, output_csv_path)

