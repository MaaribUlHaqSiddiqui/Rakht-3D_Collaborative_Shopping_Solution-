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

@app.route('/login', methods=['POST'])
def login():
    # Retrieve login credentials from request
    loginuser = request.form.get('loginuser')
    loginpass = request.form.get('loginpass')

    if not loginuser or not loginpass:
        return jsonify({"error": "Username and password are required"}), 400

    # Connect to the database
    try:
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor(dictionary=True)

        # Query to fetch the password for the given username
        query = "SELECT password FROM users WHERE username = %s"
        cursor.execute(query, (loginuser,))
        user_row = cursor.fetchone()

        if not user_row:
            return jsonify({"error": "Username does not exist"}), 404

        # Check if the provided password matches
        if user_row['password'] == loginpass:
            return jsonify({"message": "Login successful"}), 200
        else:
            return jsonify({"error": "Wrong credentials"}), 401

    except mysql.connector.Error as err:
        return jsonify({"error": str(err)}), 500
    finally:
        cursor.close()
        conn.close()

@app.route('/register', methods=['POST'])
def register():
    # Retrieve registration details from request
    registername = request.form.get('registername')
    registeruser = request.form.get('registeruser')
    registerpass = request.form.get('registerpass')

    if not registername or not registeruser or not registerpass:
        return jsonify({"error": "Name, username, and password are required"}), 400

    # Connect to the database
    try:
        conn = mysql.connector.connect(**db_config)
        cursor = conn.cursor()

        # Insert user details into the users table
        query = "INSERT INTO users (name, username, password) VALUES (%s, %s, %s)"
        cursor.execute(query, (registername, registeruser, registerpass))
        conn.commit()

        return jsonify({"message": "Registration successful"}), 201

    except mysql.connector.Error as err:
        return jsonify({"error": str(err)}), 500
    finally:
        cursor.close()
        conn.close()

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=7000)
