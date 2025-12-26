import { Toaster } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { Route, Switch, useLocation, Redirect } from "wouter";
import ErrorBoundary from "./components/ErrorBoundary";
import { ThemeProvider } from "./contexts/ThemeContext";
import Sidebar from "./components/Sidebar";
import Header from "./components/Header";
import Login from "./pages/Login";
import NotFound from "./pages/NotFound";

// Admin Pages
import Dashboard from "./pages/admin/Dashboard";
import Users from "./pages/admin/Users";
import Groups from "./pages/admin/Groups";
import Disciplines from "./pages/admin/Disciplines";
import Competencies from "./pages/admin/Competencies";
import Indicators from "./pages/admin/Indicators";
import Professions from "./pages/admin/Professions";

// Teacher Pages
import TeacherDisciplines from "./pages/teacher/Disciplines";
import Grading from "./pages/teacher/Grading";

// Student Pages
import StudentProfile from "./pages/student/Profile";
import StudentDisciplines from "./pages/student/Disciplines";
import StudentCompetencies from "./pages/student/Competencies";
import StudentProfessions from "./pages/student/Professions";

interface PageConfig {
  title: string;
  component: React.ComponentType;
  showLayout: boolean;
}

const pageConfig: Record<string, PageConfig> = {
  "/": { title: "Панель управления", component: Dashboard, showLayout: true },
  "/users": { title: "Пользователи", component: Users, showLayout: true },
  "/groups": { title: "Группы", component: Groups, showLayout: true },
  "/disciplines": { title: "Дисциплины", component: Disciplines, showLayout: true },
  "/competencies": { title: "Компетенции", component: Competencies, showLayout: true },
  "/indicators": { title: "Индикаторы", component: Indicators, showLayout: true },
  "/professions": { title: "Профессии", component: Professions, showLayout: true },
  "/teacher/disciplines": { title: "Мои дисциплины", component: TeacherDisciplines, showLayout: true },
  "/teacher/grading/:id": { title: "Оценивание студентов", component: Grading, showLayout: true },
  "/student/profile": { title: "Мой профиль", component: StudentProfile, showLayout: true },
  "/student/disciplines": { title: "Дисциплины и оценки", component: StudentDisciplines, showLayout: true },
  "/student/competencies": { title: "Освоение компетенций", component: StudentCompetencies, showLayout: true },
  "/student/professions": { title: "Мои Профессии", component: StudentProfessions, showLayout: true },
};

function Router() {
  const [location] = useLocation();
  const userRole = sessionStorage.getItem("userRole");
  
  // Redirect to login if not authenticated and not on login page
  if (!userRole && location !== "/login") {
    window.location.href = "/login";
    return null;
  }

  // Redirect to role-specific start page if on root path "/"
  if (userRole && location === "/") {
    if (userRole === "teacher") {
      return <Redirect to="/teacher/disciplines" />;
    } else if (userRole === "student") {
      return <Redirect to="/student/profile" />;
    }
    // Admin stays on "/" (Dashboard)
  }

  // Get current page config
  let currentConfig = pageConfig[location];
  if (!currentConfig) {
    // Check for dynamic routes
    for (const [path, config] of Object.entries(pageConfig)) {
      if (path.includes(":")) {
        const pattern = path.replace(/:[^/]+/g, "[^/]+");
        const regex = new RegExp(`^${pattern}$`);
        if (regex.test(location)) {
          currentConfig = config;
          break;
        }
      }
    }
  }

  const showLayout = currentConfig?.showLayout !== false && location !== "/login";
  const userName = sessionStorage.getItem("userName") || "Пользователь";

  return (
    <div className="flex">
      {showLayout && <Sidebar />}
      <div className="flex-1 flex flex-col">
        {showLayout && <Header title={currentConfig?.title || "Система управления"} userName={userName} userRole={userRole} />}
        <main className={`flex-1 ${showLayout ? "mt-20 lg:ml-60" : ""}`}>
	          <div className={showLayout ? "p-6" : ""}>
	            <Switch>
		              <Route path="/login" component={Login} />
			              {/* Admin Dashboard is the default route for admin */}
			              <Route path="/" component={Dashboard} />
		              <Route path="/users" component={Users} />
		              <Route path="/groups" component={Groups} />
		              <Route path="/disciplines" component={Disciplines} />
		              <Route path="/competencies" component={Competencies} />
			              <Route path="/indicators" component={Indicators} />
			              <Route path="/professions" component={Professions} />
		              <Route path="/teacher/disciplines" component={TeacherDisciplines} />
	              <Route path="/teacher/grading/:id" component={Grading} />
	              <Route path="/student/profile" component={StudentProfile} />
		              <Route path="/student/disciplines" component={StudentDisciplines} />
		              <Route path="/student/competencies" component={StudentCompetencies} />
		              <Route path="/student/professions" component={StudentProfessions} />
		              <Route path="/404" component={NotFound} />
	              {/* Final fallback route */}
	              <Route component={NotFound} />
	            </Switch>
	          </div>
        </main>
      </div>
    </div>
  );
}

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider defaultTheme="light">
        <TooltipProvider>
          <Toaster />
          <Router />
        </TooltipProvider>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

export default App;
