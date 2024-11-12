from flask import Flask, request, jsonify
import mysql.connector

app = Flask(__name__)

# Database connection parameters
db_config = {
    'host': 'localhost',
    'user': 'root',
    'password': '',
    'database': 'unity_backend'
}

# Route to get user measurements by username
@app.route('/get_user_measurements', methods=['GET'])
def get_user_measurements():
    # Check if username parameter is provided
    username = request.args.get('username')
    if not username:
        return jsonify({"error": "Username parameter missing"}), 400

    # Connect to the database
    try:
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor(dictionary=True)

        # Fetch userID based on username
        query_userID = "SELECT userID FROM users WHERE username = %s"
        cursor.execute(query_userID, (username,))
        user_row = cursor.fetchone()

        if not user_row:
            return jsonify({"error": f"User ID not found for username: {username}"}), 404

        userID = user_row['userID']

        # Fetch user measurements based on userID
        query_measurements = "SELECT * FROM userMeasurements WHERE userID = %s"
        cursor.execute(query_measurements, (userID,))
        measurements_row = cursor.fetchone()

        if not measurements_row:
            return jsonify({"error": f"User measurements not found for username: {username}"}), 404

        # Return user measurements in JSON format
        return jsonify(measurements_row)

    except mysql.connector.Error as err:
        return jsonify({"error": str(err)}), 500
    finally:
        cursor.close()
        conn.close()

@app.route('/get-pants', methods=['GET'])
def get_pants():
    try:
        # Connect to the database
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor(dictionary=True)

        # Fetch data from the pants table
        query = "SELECT * FROM pants"
        cursor.execute(query)
        rows = cursor.fetchall()

        if rows:
            # Return the data as JSON
            return jsonify(rows), 200
        else:
            return jsonify({"message": "0 results"}), 404

    except mysql.connector.Error as err:
        return jsonify({"error": str(err)}), 500
    finally:
        cursor.close()
        conn.close()

@app.route('/get-shirts', methods=['GET'])
def get_shirts():
    try:
        # Connect to the database
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor(dictionary=True)

        # Fetch data from the shirts table
        query = "SELECT * FROM shirts"
        cursor.execute(query)
        rows = cursor.fetchall()

        if rows:
            # Return the data as JSON
            return jsonify(rows), 200
        else:
            return jsonify({"message": "0 results"}), 404

    except mysql.connector.Error as err:
        return jsonify({"error": str(err)}), 500
    finally:
        cursor.close()
        conn.close()

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=6000)
