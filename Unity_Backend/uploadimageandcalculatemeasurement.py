from flask import Flask, request, jsonify
import mysql.connector
import os
import sys

# Initialize Flask app
app = Flask(__name__)

# Database connection parameters
db_config = {
    'host': 'localhost',
    'user': 'root',
    'password': '',
    'database': 'unity_backend'
}

@app.route('/upload-image', methods=['POST'])
def upload_image():
    username = request.form.get("username")
    image_path = request.form.get("imagePath")

    if not username or not image_path:
        return jsonify({"error": "Username and image path are required"}), 400

    try:
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor(dictionary=True)

        # Get UserID based on the provided Username
        query = "SELECT UserID FROM users WHERE Username = %s"
        cursor.execute(query, (username,))
        user_row = cursor.fetchone()

        if not user_row:
            return jsonify({"error": "User not found with the provided username"}), 404

        user_id = user_row["UserID"]

        # Check if the user already has an entry in UserImages table
        check_query = "SELECT ImageID FROM users_images WHERE UserID = %s"
        cursor.execute(check_query, (user_id,))
        existing_image = cursor.fetchone()

        if existing_image:
            # Update the existing record and delete previous image path
            update_query = "UPDATE users_images SET ImagePath = %s WHERE UserID = %s"
            delete_previous_query = "DELETE FROM users_images WHERE UserID = %s AND ImagePath != %s"
            cursor.execute(update_query, (image_path, user_id))
            cursor.execute(delete_previous_query, (user_id, image_path))
            conn.commit()
            return jsonify({"message": "Image path updated successfully"}), 200
        else:
            # Insert a new record if no entry exists
            insert_query = "INSERT INTO users_images (UserID, ImagePath) VALUES (%s, %s)"
            cursor.execute(insert_query, (user_id, image_path))
            conn.commit()
            return jsonify({"message": "Image path saved successfully"}), 201

    except mysql.connector.Error as err:
        return jsonify({"error": str(err)}), 500
    finally:
        cursor.close()
        conn.close()

@app.route('/calculate_measurements', methods=['POST'])
def calculate_measurements():
    if request.method == 'POST' and 'imagePath' in request.form and 'height' in request.form and 'username' in request.form:
        imagePath = request.form['imagePath']
        height = request.form['height']
        username = request.form['username']

        # Import the required function from measurement_script.py
        from measurement_script import calculate_measurements

        # Calculate measurements
        measurements = calculate_measurements(imagePath, height)

        try:
            conn = mysql.connector.connect(**db_config)
            cursor = conn.cursor()

            # Fetch userID based on username
            cursor.execute("SELECT userID FROM users WHERE username = %s", (username,))
            result = cursor.fetchone()
            if result:
                userID = result[0]
            else:
                cursor.close()
                conn.close()
                return "Error: User does not exist", 404

            # Check if measurements already exist for the provided userID
            cursor.execute("SELECT userMeasurementID FROM UserMeasurements WHERE userID = %s", (userID,))
            existing_measurements = cursor.fetchall()

            if existing_measurements:
                # If measurements exist, delete them
                delete_query = "DELETE FROM UserMeasurements WHERE userID = %s"
                cursor.execute(delete_query, (userID,))
                conn.commit()

            # Insert new measurement data
            insert_query = """
                INSERT INTO UserMeasurements (userID, distance_shoulder, distance_waist, distance_arms, distance_arms2, distance_legs)
                VALUES (%s, %s, %s, %s, %s, %s)
            """
            insert_values = (
                userID,
                measurements['Distance Shoulder'],
                measurements['Distance Waist'],
                measurements['Distance Arms'],
                measurements['Distance Arms2'],
                measurements['Distance Legs']
            )
            cursor.execute(insert_query, insert_values)
            conn.commit()

            return "Measurements received and stored successfully", 200

        except mysql.connector.Error as err:
            return jsonify({"error": str(err)}), 500
        finally:
            cursor.close()
            conn.close()
    else:
        return "Error: Invalid request", 400

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
