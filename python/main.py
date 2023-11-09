import socket
import json
from live_advance import LiveAdvance


your_app_client_id = 'ZVB8dyW0Hy29quBcLnYkSMG1ZD2piJ3slWwHYiz7'
your_app_client_secret = 'OBKmO3Ppd7dymE4PCtmUC8yJQMY5qKBCkrKQAMwd8QxTQT4jZU5Sqiv24VRQsXlprXa4KAF5boxH5Tsak6WB4TK1IyiIqSwxKoZWZKK1fZCgV54T51aZbufvSDcr75eJ'

# Init live advance
l = LiveAdvance(your_app_client_id, your_app_client_secret)

trained_profile_name = 'kyleSIM' # Please set a trained profile name here
l.start(trained_profile_name)