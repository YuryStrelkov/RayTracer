clear all;
clc;

move = [1 2 3];

Move  = [[1 0 0 move(1)];...
         [0 1 0 move(2)];...
         [0 0 1 move(3)];
         [0 0 0 1]];
forward = move/norm(move);

right = cross([0 1 0],forward);

right = right/norm(right);

up =  cross(forward,right);

view = [[right' up' forward' [0 0 0]'];...
        [0 0 0 1]];