classDiagram
    %% ==========================================
    %% CORE SYSTEMS & FLOW
    %% ==========================================
    class GameManager {
        +ResetProgress()
    }
    GameManager --> WaveManager : waveManager
    
    class WaveManager {
        +int CurrentWave
        +int ClearedWaves
        +float ReadyTime
        +float WaveDuration
        +float RemainingTime
        +SetWaveDuration(duration)
        +IsPlanning() bool
        +IsPlaying() bool
        +IsUpgrading() bool
        +IsGameOver() bool
        +StartPlanningPhase()
        +UpdatePlanningTimer()
        +StartPlayingPhase()
        +UpdatePlayingTimer()
        +StartUpgradePhase()
        +HandleGameOver()
        +ContinueAfterGameOver()
    }
    WaveManager *-- WaveStateMachine : stateMachine
    WaveManager --> SpawnManager : spawnManager
    WaveManager --> Dessert : dessert
    WaveManager --> GameOverUI : gameOverUI
    WaveManager --> ExperienceManager : experienceManager
    WaveManager --> VictoryTracker : victoryTracker
    WaveManager --> PlayerUpgradeStats : playerUpgradeStats
    
    class WaveStateMachine {
        +ChangeState(IWaveState newState)
        +Update()
        +IsInState() bool
    }
    WaveStateMachine --> IWaveState : CurrentState
    
    class IWaveState {
        <<interface>>
        +Enter()
        +Update()
        +Exit()
    }
    
    class GameOverState
    class PlanningState
    class PlayingState
    class UpgradeState
    
    IWaveState <|.. GameOverState
    IWaveState <|.. PlanningState
    IWaveState <|.. PlayingState
    IWaveState <|.. UpgradeState
    
    GameOverState --> WaveManager : waveManager
    PlanningState --> WaveManager : waveManager
    PlayingState --> WaveManager : waveManager
    UpgradeState --> WaveManager : waveManager
    
    class VictoryTracker {
        +Reset()
        +OnAntSpawned()
        +OnAntDeath(ant, expValue)
        +OnSpawnComplete()
    }
    
    class SpawnManager {
        +ResetProgress()
        +StartWave()
        +ClearPreviousWave()
        +ExpandSpawnArea(amount)
        +SetSpawnArea(xMin, xMax, yMin, yMax)
    }
    SpawnManager *-- NestPositionCalculate : positionCalculator
    
    class NestPositionCalculate {
        +UpdateArea(xMin, xMax, yMin, yMax)
        +TryGetNestPosition(existingPositions, out position) bool
    }
    
    class MapExpander {
    }
    MapExpander --> SpawnManager : spawnManager
    MapExpander --> WaveManager : waveManager

    %% ==========================================
    %% ENTITIES (Ants & Dessert)
    %% ==========================================
    class IDamageable {
        <<interface>>
        +TakeDamage(amount)
    }
    
    class IKnockbackable {
        <<interface>>
        +ApplyKnockback(impulse)
    }
    
    class Ant {
        +bool isKnockedBack
        +ApplyKnockback(impulse)
        +TakeDamage(amount)
        +ResetForPool()
    }
    IDamageable <|.. Ant
    IKnockbackable <|.. Ant
    Ant --> AntStackController : antStacker
    Ant --> TNTPlacementController : tntController
    
    class AntMovement {
        +SetPathingEnabled(enabled)
        +ResetMovement()
    }
    AntMovement --> Ant : ant
    AntMovement --> AntLineController : antLineController
    AntMovement --> AntStackController : stacker
    AntMovement --> AntStats : antStats
    
    class AntStackController {
        +GetExpMultiplier() float
        +DetachFromStack()
        +LeaveStack()
    }
    AntStackController --> Ant : ant
    AntStackController --> AntMovement : antMovement
    AntStackController --> Ant : StackedWith
    
    class AntStats {
        +float MaxHealth
        +float MoveSpeed
        +int DamageToDessert
        +float TntResistance
        +ResetStats()
    }
    AntStats --> AntType : AntType
    
    class AntEating {
    }
    AntEating --> AntStats : antStats
    AntEating --> Dessert : targetDessert
    
    class AntLineController {
        +UpdatePosition()
        +OnReachedDessert(ant)
        +RemoveAnt(ant)
    }
    
    class AntType {
        <<enumeration>>
        Normal
        Fast
        Tank
        HeavyDamage
        Chimera
    }
    
    class Dessert {
        +float CurrentHealth
        +float MaxHealth
        +ResetHealth()
        +TakeDamage(damage)
    }
    IDamageable <|.. Dessert
    Dessert --> PlayerUpgradeStats : playerUpgradeStats
    
    class DessertVisualController {
    }
    DessertVisualController --> Dessert : dessert

    %% ==========================================
    %% WEAPONS / TNT
    %% ==========================================
    class TNTPlacementController {
        +int RemainingTNT
        +RaiseExplosion(position, radius, damage)
        +RaiseShockwave(position, radius, force)
        +GetChainStart() TNTLogic
    }
    TNTPlacementController --> TNTLogic : tntPrefab / placedTNTs
    TNTPlacementController --> ExplosionRadiusIndicator : previewIndicatorPrefab
    TNTPlacementController --> FuseConnection : fuseLinePrefab / placedFuses
    TNTPlacementController --> PlayerUpgradeStats : playerUpgradeStats
    TNTPlacementController --> WaveManager : waveManager
    
    class TNTLogic {
        +float BaseExplosionRadius
        +float ExplosionRadius
        +float ShockwaveRadius
        +Initialize(PlayerUpgradeStats, TNTPlacementController)
        +SetNext(TNTLogic next, distance)
        +Ignite()
    }
    TNTLogic --> PlayerUpgradeStats : playerStats
    TNTLogic --> TNTPlacementController : placementController
    TNTLogic --> TNTLogic : nextInChain
    
    class TNTVisual {
    }
    TNTVisual --> ExplosionRadiusIndicator : blastRadiusPrefab
    TNTVisual --> ShockwaveEffect : shockwavePrefab
    TNTVisual --> TNTLogic : logic
    
    class FuseConnection {
        +Setup(TNTLogic fromTNT, to) float
    }
    FuseConnection --> TNTLogic : from
    
    class ExplosionRadiusIndicator {
        +SetRadius(radius)
        +SetVisible(visible)
    }
    
    class ShockwaveEffect {
        +Play(startRadius, endRadius, duration)
    }
    
    class Detonator {
        +DetonateChain()
    }
    Detonator --> TNTPlacementController : placementController

    %% ==========================================
    %% UPGRADES & PROGRESSION
    %% ==========================================
    class ExperienceManager {
        +int CurrentLevel
        +int PendingLevelUps
        +float CurrentXP
        +float XPRequiredForNextLevel
        +ResolveWaveEnd()
        +ConsumePendingLevelUp()
        +ResetProgress()
    }
    
    class PlayerUpgradeStats {
        +float BonusExplosionRadius
        +int BonusMaxTNTCount
        +float BonusKnockbackForce
        +float BonusFuseBurnSpeed
        +float BonusMaxFuseDistance
        +float BonusMaxDessertHealth
        +AddBonus(UpgradeStatType statType, amount)
        +GetBonus(UpgradeStatType statType) float
        +ResetBonuses()
    }
    
    class UpgradeContext {
    }
    UpgradeContext --> PlayerUpgradeStats : PlayerStats
    UpgradeContext --> Dessert : Dessert
    
    class UpgradeDefinition {
        <<abstract>>
        +string Title
        +string Description
        +Apply(UpgradeContext context)*
    }
    
    class PercentMaxHealthUpgrade {
        +Apply(UpgradeContext context)
    }
    UpgradeDefinition <|-- PercentMaxHealthUpgrade
    
    class StatUpgradeDefinition {
        +Apply(UpgradeContext context)
    }
    UpgradeDefinition <|-- StatUpgradeDefinition
    StatUpgradeDefinition --> UpgradeStatType : statType
    
    class UpgradeStatType {
        <<enumeration>>
        ExplosionRadius
        MaxTNTCount
        KnockbackForce
        FuseBurnSpeed
        MaxFuseDistance
        MaxDessertHealth
    }

    %% ==========================================
    %% UI MANAGERS & COMPONENTS
    %% ==========================================
    class MenuUIHandler {
        +PlayClicked()
        +VolumeClicked()
        +HowToPlayClicked()
        +CreditsClicked()
        +BackClicked()
        +QuitClicked()
    }
    
    class UIManager {
        +ShowPause()
        +ResumeGame()
        +ShowMenu()
    }
    UIManager --> WaveManager : waveManager
    
    class GameOverUI {
        +Show()
        +RetryWave()
        +ReturnToMainMenu()
    }
    GameOverUI --> WaveManager : waveManager
    
    class UpgradeSelectionUI {
    }
    UpgradeSelectionUI --> ExperienceManager : experienceManager
    UpgradeSelectionUI --> PlayerUpgradeStats : playerUpgradeStats
    UpgradeSelectionUI --> Dessert : dessert
    UpgradeSelectionUI --> UpgradeContext : context
    UpgradeSelectionUI --> UpgradeChoiceButtonUI : spawnedButtons
    UpgradeSelectionUI --> UpgradeDefinition : upgradePool
    
    class UpgradeChoiceButtonUI {
        +Setup(UpgradeDefinition, Action)
    }
    UpgradeChoiceButtonUI --> UpgradeDefinition : upgrade
    
    class XPUI {
    }
    XPUI --> ExperienceManager : experienceManager
    
    class WaveTimerUI {
    }
    WaveTimerUI --> WaveManager : waveManager
    WaveTimerUI --> IWaveState : currentState

    %% ==========================================
    %% AUDIO & SIGNALS (STATIC EVENTS)
    %% ==========================================
    class AudioManager {
        +PlayMusic(clip)
        +StopMusic()
        +PlaySfx(clip)
        +PlayEating(clip)
        +StopEating()
        +SetMusicVolume(volume)
        +SetSFXVolume(volume)
        +PauseAll()
        +ResumeAll()
    }
    
    class DessertEatingSound {
    }
    DessertEatingSound --> AudioManager : audioManager
    
    class ExplosionSound {
    }
    ExplosionSound --> TNTPlacementController : tntController
    
    class UpgradeSound {
    }
    UpgradeSound --> AudioManager : audioManager
    
    class WaveMusicController
    
    class DessertDestroyedSignal {
        <<static>>
        +Raise()
    }
    class DessertEatingSignal {
        <<static>>
        +RaiseEatingStarted()
        +RaiseEatingStopped()
    }
    class UpgradeChosenSignal {
        <<static>>
        +Raise()
    }
