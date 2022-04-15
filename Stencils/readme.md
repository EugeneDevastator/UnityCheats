# Stencil Cheat Page

Two web pages here have hints about how stencil works.

Most brilliant idea is that it can be used as 8 separate masks using masking, or as one continuous mask.
https://htmlpreview.github.io/?https://github.com/EugeneDevastator/UnityCheats/blob/main/Stencils/OperationsTable.html
https://htmlpreview.github.io/?https://github.com/EugeneDevastator/UnityCheats/blob/main/Stencils/StencilComparison.html

## Things to keep in mind in regards to ui:

1. Unity's Mask component always writes all bits so.
2. Unity's rect mask doesnt use stencil at all and relies on rect parameter for shader. Effectively only narrowing rect down for nested elements.
3. It al can be worked around with separate canvas though at a cost of changing Render target.
