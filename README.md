
Lien de téléchargement du jeu : https://drive.google.com/open?id=1J4xCIYol7AKrL73ucbJ_oGW-Yzzh5COf
Sur Unity 2019.1

Il vous suffit d'ajouter le dossier contenant les fichiers de ce git dans Unity Hub en temps que nouveau projet pour explorer CodeMiner
Les images sont dans Asset/Ressources 
Les scripts sont dans Asset/Script


L’objectif de ce travail était de développer une application qui introduit la programmation, ou du
moins la logique algorithmique, d’une façon ludique. Le concept est inspiré d’un travail appelé Codeweaver, réalisé par
deux japonais Ren SAKAMOTO et Toshikazu OHSHIMA en 2018. Le but de leur application
est de donner des commandes pour contrôler une araignée vue de dessus, qui trace une
ligne derrière elle, l’objectif étant de dessiner des motifs.
Dans une optique d’originalité et d’authenticité, le but du jeu a été modifié comme cela : il
faut déplacer un personnage pour récupérer toutes les pépites d’or sur le niveau. Il existe
plusieurs niveaux, qui ont une difficulté croissante, qu’il est possible d’accéder en
réussissant le précédent, ou tout simplement grâce aux commandes prévues.

C’est dans la façon dont les commandes sont données au personnage que réside l’originalité
de l’application : il faut simplement organiser des cartes physiques dans l’ordre voulu, qu’une
caméra captera et que le programme interprétera. Une carte correspond à une commande,
c’est-à-dire que la carte « Avancer » fera déplacer le personnage d’une case vers l’avant.
Les trois fonctions de mouvements sont : avancer, tourner à droite, tourner à gauche.

Tel un programme, il faut mettre les fonctions les unes à la suite des autres, de haut en bas,
et finir par la carte « Goal », qui annoncera le top-départ. Si le personnage n’a pas récupéré
toutes les pépites avant la fin de ses mouvements, il revient à son point de départ et les
pépites réapparaissent. Le but est donc de faire tout le chemin d’une seule traite, avec
seulement quelques cartes.

Pour éviter d’avoir à mettre trop de cartes, il est possible de mettre un « argument » à
chaque fonction. Les arguments sont des cartes comme les autres fonctions, réparties entre
2 et 6. Ces numéros indiquent le nombre de fois qu’une fonction sera répétée. Par exemple,
si l’on met la carte argument « 2 » à côté de la fonction « avancer », le personnage avancera
de 2 cases.

Afin de complexifier le jeu et permettre une plus grande liberté, il existe des fonctions de
« boucles », à la manière d’algorithme. Toutes les fonctions disposées entre le début et la fin
d’une boucle, seront répétées. A la manière des autres fonctions, les boucles peuvent
prendre des arguments.

Grâce à toutes ces fonctionnalités, il est possible de déplacer le personnage de multiples
manières, la seule limite étant le champ de vision de la caméra, le nombre de cartes, et notre
imagination.
Ainsi, il existe 10 niveaux différents plus ou moins difficiles, mais tous réalisables.
