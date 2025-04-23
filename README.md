# StrategoGameServer
A simple game server for a multiplayer Stratego application. This server is in charge of account persistence, stats, and matchmaking.

# StrategoGameServer API Documentation

Explore the available endpoints for the StrategoGameServer API.

## Endpoints

### HomeController
- **GET /**  
  Serves the `map.html` file as the homepage.

### AuthController
- **POST /api/auth/login**  
  Logs in a user with their credentials.

- **POST /api/auth/register**  
  Registers a new user account.

- **POST /api/auth/logout**  
  Logs out a currently authenticated user.

- **GET /api/auth/concurrentUsers**  
  Retrieves a list of currently authenticated users.

- **GET /api/auth/getAccount?username=**  
  Retrieves account details for a specific username.

- **GET /api/auth/logoutAll**  
  Logs out all currently authenticated users.

### GameController
- **GET /api/game/getGames**  
  Retrieves a list of all active games.

- **POST /api/game/findGame**  
  Attempts to find a game for the user.

- **POST /api/game/postMove**  
  Submits a move for the current game.

- **DELETE /api/game/endGame**  
  Ends the current game session.

- **POST /api/game/getBoard**  
  Retrieves the game board for a specific lobby and color.

- **GET /api/game/ResetBoard**  
  Resets the game board to its initial state.