SELECT * FROM sqlgraph.Employee
INNER JOIN sqlgraph.employee_group ON sqlgraph.employee_group.employeeID = sqlgraph.Employee.ID 
INNER JOIN sqlgraph.Group ON sqlgraph.Employee_Group.GroupID = sqlgraph.Group.ID
WHERE sqlgraph.group.Name = 'Sales'

#the gremlin equivalent :-)
# g.V('Sales').out('member')
