# jwt-refresh-token
dependencies: .net core 3.1.10
Database is configured with demo, it has configured, you can download and start app

Links : 
# Default: https://localhost:44318/Login/Login
# Create User | Signup | Register : https://localhost:44318/Users/Create
    # if not logged in then it should response following :
    {
        "message": "Unauthorized"
    }
    # if logged in then, response should be user 
    
    
# List Of Users : https://localhost:44318/User/Index

# Token refresh only when user click login button

# User SQL Query

# CREATE TABLE Users (
    id uuid DEFAULT uuid_generate_v4 (), 
    created date, 
    modified date,
    username varchar,
    password varchar,
    name varchar,
    email varchar,
    salt varchar,
    refresh_token varchar,
    token_expiration date,
    refresh_token_expiration date 
); # 
