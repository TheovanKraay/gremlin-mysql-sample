SELECT sqlgraph.group.ID, sqlgraph.group.Name FROM sqlgraph.group
INNER JOIN sqlgraph.groups
ON GroupID = sqlgraph.group.ID
WHERE NestedGroupID = 
(SELECT ID FROM sqlgraph.group WHERE Name='Azure')

#the gremlin equivalent :-)
# g.V('Azure').out('subgroup')
