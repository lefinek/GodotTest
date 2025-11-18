extends Control

# Scene paths
const LOBBY_CREATE_PATH = "res://Scenes/LobbyCreate/LobbyCreate.tscn"
const LOBBY_SEARCH_PATH = "res://Scenes/LobbySearch/LobbySearch.tscn"
var eos_manager: EOSManager

func _ready() -> void:
	# Connect button signals
	var create_button = $Panel/MenuCenter/VMenu/CreateGame/CreateGameButton
	var join_button = $Panel/MenuCenter/VMenu/JoinGame/JoinGameButton
	var quit_button = $Panel/MenuCenter/VMenu/Quit/QuitButton
	eos_manager = get_node("/root/EOSManager")
	
	create_button.pressed.connect(_on_create_game_pressed)
	join_button.pressed.connect(_on_join_game_pressed)
	quit_button.pressed.connect(_on_quit_pressed)

func _on_create_game_pressed() -> void:
	print("Loading Lobby Create scene...")
	
	# Opuść obecne lobby jeśli jesteś w jakimś
	if eos_manager != null and eos_manager.currentLobbyId != "":
		print("🚪 Leaving current lobby before switching scenes...")
		eos_manager.LeaveLobby()
	
	# Przejdź do sceny tworzenia lobby (lobby zostanie utworzone TAM, nie tutaj)
	get_tree().change_scene_to_file(LOBBY_CREATE_PATH)


func _on_join_game_pressed() -> void:
	print("Loading Lobby Search scene...")
	# use constant path to switch scenes
	get_tree().change_scene_to_file(LOBBY_SEARCH_PATH)

func _on_quit_pressed() -> void:
	print("Quitting game...")
	get_tree().quit()


func _on_create_game_button_pressed() -> void:
	pass # Replace with function body.
