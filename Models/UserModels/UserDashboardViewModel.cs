public class UserDashboardViewModel
{
    public string UserName { get; set; }
    public List<ProjectAssignments> ProjectAssignments { get; set; }
}

public class ProjectAssignments
{
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
    public List<AssignmentInfo> Assignments { get; set; }
}

public class AssignmentInfo
{
    public string AssignmentName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
