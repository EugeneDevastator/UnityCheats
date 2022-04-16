# Scriptable object duplication.

Circumstances: we have one SO (used by many systems) is also in addressables.

## What will happen: 

1. every user who uses direct link will get copy A.
2. every user who tries to get SO from addressables will get copy B.
3. Some object is being created via addressables. Even if it has direct link, this link will lead to addressable copy B.



