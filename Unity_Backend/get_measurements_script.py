# import subprocess
# import mysql.connector
# import time

# # Connect to the database
# db_connection = mysql.connector.connect(
#     host="localhost",
#     user="root",
#     password="",
#     database="unity_backend"
# )

# # Check the database connection
# if db_connection.is_connected():
#     print("Connected to the database")
# else:
#     print("Failed to connect to the database")
#     exit()

# # Create a cursor to execute SQL queries
# cursor = db_connection.cursor()

# # Infinite loop to continuously check for new images
# while True:
#     try:
#         # Query for the latest image path that hasn't been processed
#         query = "SELECT UserID, ImagePath FROM users_images WHERE UserID = 2"
#         cursor.execute(query)
#         result = cursor.fetchone()

#         if result:
#             user_id, image_path = result

#             # Run the body measurement script as a subprocess
#             process = subprocess.Popen(["python", "body_measurement_script.py", image_path], stdout=subprocess.PIPE, text=True)
#             output, _ = process.communicate()

#             # Parse the measurements from the subprocess output
#             measurements = {}
#             for line in output.split('\n'):
#                 if line:
#                     key, value = line.split(':')
#                     measurements[key.strip()] = float(value.strip())

#             # Store the measurements into a new table (e.g., MeasurementsTable)
#             insert_query = "INSERT INTO MeasurementsTable (UserID, DistanceShoulder, DistanceWaist, DistanceArms, DistanceArms2, DistanceLegs) VALUES (%s, %s, %s, %s, %s, %s)"
#             cursor.execute(insert_query, (user_id, measurements.get('Distance Shoulder', 0), measurements.get('Distance Waist', 0), measurements.get('Distance Arms', 0), measurements.get('Distance Arms2', 0), measurements.get('Distance Legs', 0)))
#             db_connection.commit()

#             # Mark the image as processed in the UserImages table
#             update_query = "UPDATE UserImages SET Processed = 1 WHERE UserID = %s AND ImagePath = %s"
#             cursor.execute(update_query, (user_id, image_path))
#             db_connection.commit()

#             print(f"Measurements stored for UserID {user_id}, ImagePath: {image_path}")

#         # Sleep for a while before checking for new images again
#         time.sleep(60)  # Adjust the interval as needed

#     except KeyboardInterrupt:
#         # Exit gracefully on keyboard interrupt
#         break

# # Close the database connection
# cursor.close()
# db_connection.close()

