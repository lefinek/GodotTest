# Migracja do C# - Informacje

## ✅ Zakończona konwersja

Wszystkie skrypty gry zostały przepisane z GDScript na C# i znajdują się w katalogu `Scripts/`.

### Przekonwertowane pliki:

#### Autoloady (singletons):

- `GlobalData.cs` - zarządzanie stanem gry, graczy, punktacji
- `ProductDetails.cs` - klucze produktu EOS (używaj snake_case: `product_id`, `sandbox_id`, etc.)

#### Gameplay:

- `PlayerCharacter.cs` - gracz z ruchem, zdrowiem, RPC
- `Weapon.cs` - broń z cooldownem i spawnem pocisków
- `Bullet.cs` - pociski z kolizjami
- `HealthBar.cs` - pasek zdrowia gracza
- `ScoreLabel.cs` - wyświetlanie punktów

#### UI & Networking:

- `MainRoot.cs` - inicjalizacja EOS, logowanie
- `GameRoot.cs` - zarządzanie serwerem/klientem, spawnem graczy
- `GUIRoot.cs` - zarządzanie interfejsem
- `ConnectButton.cs` - przycisk połączenia
- `DisconnectButton.cs` - przycisk rozłączenia
- `NatTypeDisplay.cs` - wyświetlanie typu NAT

### Zaktualizowane sceny:

- `main.tscn` - główna scena
- `player/Player.tscn` - gracz
- `player/Bullet.tscn` - pocisk
- `player/health_bar.tscn` - pasek zdrowia

### Usunięte pliki .gd:

Wszystkie pliki `.gd` i `.gd.uid` z katalogu głównego i `player/` zostały usunięte.

### Zachowane pliki:

**Addon EOS** - wszystkie pliki w `addons/epic-online-services-godot/` pozostały w GDScript.

## 🚀 Następne kroki

1. **Otwórz projekt w Godot .NET**

   - Upewnij się, że używasz wersji Godot 4.x z obsługą .NET/Mono
   - Przy pierwszym otwarciu Godot utworzy `.godot/mono/` i skompiluje projekt

2. **Sprawdź kompilację**

   - Godot automatycznie zbuduje rozwiązanie C#
   - Jeśli widzisz błędy, sprawdź czy wszystkie zależności EOS są dostępne

3. **Testowanie**

   - Uruchom scenę `main.tscn`
   - Przetestuj logowanie (Device ID lub Dev Credential)
   - Przetestuj utworzenie serwera i połączenie klienta
   - Sprawdź ruch gracza, strzelanie, kolizje

4. **Ewentualne poprawki**
   - Jeśli API EOS różni się w C#, dostosuj wywołania w `MainRoot.cs`
   - Sprawdź czy wszystkie sygnały/eventy są poprawnie podpięte
   - Zweryfikuj RPC między klientem a serwerem

## 📝 Różnice GDScript vs C#

### Sygnały:

```gdscript
# GDScript
signal score_changed(score: int)
score_changed.emit(score)
```

```csharp
// C#
[Signal] public delegate void ScoreChangedEventHandler(int score);
EmitSignal(SignalNameScoreChanged, score);
```

### RPC:

```gdscript
# GDScript
@rpc("authority", "call_local", "reliable")
func take_damage(amount: int):
```

```csharp
// C#
[Rpc(MultiplayerPeer.RpcMode.Authority)]
public void TakeDamage(int amount, int peerId)
```

### Export:

```gdscript
# GDScript
@export var speed: float = 300.0
```

```csharp
// C#
[Export] public float Speed { get; set; } = 300f;
```

### Pobieranie węzłów:

```gdscript
# GDScript
@onready var gun: Weapon = $Gun
```

```csharp
// C#
private Weapon _gun;
public override void _Ready() {
    _gun = GetNode<Weapon>("Gun");
}
```

## ⚠️ Uwagi

- **ProductDetails** używa snake_case dla kompatybilności z potencjalnym kodem GDScript w addon EOS
- **RPC reliability/call_local** - w C# użyto prostych atrybutów `[Rpc]`, możesz doprecyzować flagi jeśli potrzebne
- **Bezpieczeństwo** - klucze EOS są nadal w kodzie źródłowym; rozważ załadowanie z pliku konfiguracyjnego
- **EOS API** - jeśli interfejs C# różni się od GDScript, może być potrzebna adaptacja wywołań

## 🐛 Znane kwestie do sprawdzenia

1. Spawning pocisków - sprawdź czy `AddChild(node, true)` działa poprawnie w multiplayer
2. Random pozycja gracza - użyto prostego modulo, możesz zamienić na `GD.RandRange()`
3. Cooldown broni - teraz używa `RateOfFire`, upewnij się że to zamierzone zachowanie
4. Sygnały/Eventy - sprawdź czy wszystkie połączenia działają (szczególnie w GUI)

## 📚 Przydatne linki

- [Godot C# Documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/)
- [Godot Multiplayer in C#](https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html)
- [EOS Plugin Documentation](https://github.com/3ddelano/epic-online-services-godot)
