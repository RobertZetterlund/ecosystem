# Simulating an ecosystem (Group 89)
![](https://img.shields.io/badge/DATX02-Group%2089-green)[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=flat&logo=unity)](https://unity3d.com)
![](https://img.shields.io/badge/Made%20in-Visual%20Studio-blueviolet)

### Exploring the possibility of generating terrain-dependent non-player character behaviour by using an evolutionary-based fuzzy cognitive map

*The purpose of this project was to investigate if Fuzzy Cognitive Maps could be used in combination with Procedural Content Generation, to provide an ecosystem with evolving dynamic behaviour among NPCs in game development.*

<div>
<p align="center">
<img src="https://user-images.githubusercontent.com/31474146/85918404-c636b000-b862-11ea-8791-d5ea26e46dba.png" width="45%" height="45%">
</p>
  <p align="center">
    Screenshot of our ecosystem. Rabbits survive by eating plants, drinking water and escaping foxes.
  </p>
</div>


### Abstract 

Providing immersive experiences in video games often proves to be difficult as rudimentary Non-Playable Characters (NPCs) can appear repetitive to players. In an attempt to address this issue, we used the game engine Unity to implement a behavioral model using evolving Fuzzy Cognitive Maps (FCMs). An FCM is a weighted directed graph where nodes represent concepts and edges determine their causality. Through simulations in a procedural generated environment and usage of genetic algorithms, we sought a correlation between the decision-making of animals and their environment. As the animals' FCMs processed input from the environment, their decision making developed throughout several generations. Simulations, done with three different environments, have shown that animals simulated in hostile environments evolved to prioritise certain actions such as escaping as opposed to those in peaceful environments. Evolved FCMs consistently performed better than their predecessors and we concluded that our model could be used to successfully create environment dependent decision making of NPCs in a video game setting.

[**Read the thesis**](Ecosystem_Thesis.pdf)
