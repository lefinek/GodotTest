extends Control

var eos_manager: EOSManager

func _ready() -> void:
	eos_manager = get_node("/root/EOSManager")
	
	# Podłącz przycisk ustawiania nicku
	var set_nick_button = $Panel/NicknamePanel/SetNicknameButton
	set_nick_button.pressed.connect(_on_set_nickname_pressed)
	
	# Utwórz lobby po wejściu na scenę (z retry jeśli EOS się jeszcze nie zalogował)
	create_lobby_with_retry()

func _on_set_nickname_pressed() -> void:
	var nickname_edit = $Panel/NicknamePanel/NicknameEdit
	var nickname = nickname_edit.text.strip_edges()
	if nickname != "":
		eos_manager.SetPendingNickname(nickname)
		print("✅ Nickname set: ", nickname)
	else:
		print("⚠️ Nickname is empty")

func _on_back_button_pressed() -> void:
	print("Returning to main menu...")
	# Opuść lobby jeśli jesteś w jakimś
	if eos_manager != null and eos_manager.currentLobbyId != "":
		print("🚪 Leaving lobby before returning to menu...")
		eos_manager.LeaveLobby()
	get_tree().change_scene_to_file("res://Scenes/MainMenu/main.tscn")


func _on_leave_lobby_pressed() -> void:
	print("Returning to main menu...")
	# Opuść lobby jeśli jesteś w jakimś
	if eos_manager != null and eos_manager.currentLobbyId != "":
		print("🚪 Leaving lobby before returning to menu...")
		eos_manager.LeaveLobby()
	get_tree().change_scene_to_file("res://Scenes/MainMenu/main.tscn")


func create_lobby_with_retry(attempt: int = 0) -> void:
	# Sprawdź czy użytkownik jest już zalogowany
	if eos_manager == null:
		print("⚠️ EOSManager not found, retrying in 0.5s...")
		await get_tree().create_timer(0.5).timeout
		create_lobby_with_retry(attempt + 1)
		return
	
	# Sprawdź czy już nie ma lobby (np. powrót z innej sceny)
	if eos_manager.currentLobbyId != null and eos_manager.currentLobbyId != "":
		print("✅ Already in lobby: ", eos_manager.currentLobbyId)
		return
	
	# Sprawdź czy EOS jest zalogowany
	if not eos_manager.IsLoggedIn():
		if attempt < 10:
			print("⏳ Waiting for EOS login... (attempt %d/10)" % (attempt + 1))
			await get_tree().create_timer(0.5).timeout
			create_lobby_with_retry(attempt + 1)
		else:
			print("❌ EOS login timeout - could not create lobby")
		return
	
	# Teraz możemy bezpiecznie utworzyć lobby
	print("✅ EOS logged in, creating lobby...")
	eos_manager.CreateLobby("MyLobby", 4, true)
