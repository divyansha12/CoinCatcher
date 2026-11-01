# Coin Catcher

A 2D Unity game where you move a bucket to catch falling coins and avoid bombs.

## Demo Video
[Watch the gameplay recording](https://drive.google.com/file/d/1W2quKxblWRAiTU1Szf0db9eVbGZNnl_q/view?usp=sharing)

## Goal
Collect as many coins as you can and avoid the bombs. The game ends when you catch 3 bombs or the 60-second timer runs out.

## Game Rules
- **Coins** fall from the top at random positions. Catching one increases your score.
- **Bombs** fall the same way. Catching one costs 1 life and flashes the bucket red.
- **Game Over** happens after 3 bombs or when the timer hits zero.
- Missing a coin costs no life, but it resets your combo.

## Features
- Score, 60-second timer and 3 lives
- Gold (10), silver (5) and bonus (25) coins
- Combo multiplier up to x4 for catching coins in a row
- 3 difficulty stages: coins fall faster and bombs appear more often
- Best score saved between plays
- Sunset background with stars (change `theme` in GameManager.cs for ocean or forest)

## Controls
- Left/Right arrow keys or A/D to move the bucket
- Or hold the mouse button and drag
- Press R after Game Over to restart

## Scripts
- `BucketController.cs` moves the bucket and detects catches
- `CoinSpawner.cs` generates coins and bombs, with the bomb ratio rising each stage
- `Coin.cs` handles falling objects, which can be a coin or a bomb
- `GameManager.cs` handles score, lives, timer, stages, HUD and Game Over

## How to run
1. Install Unity 6 using Unity Hub.
2. Clone this repo and open the folder in Unity Hub.
3. Open Assets/Scenes/SampleScene and press Play.
