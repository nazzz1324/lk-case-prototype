import { useState } from "react";
import { Link, useLocation } from "wouter";
import {
  Home,
  Users,
  UsersIcon,
  BookOpen,
  Target,
  Pin,
  Link as LinkIcon,
  Briefcase,
  LogOut,
  Menu,
  X,
} from "lucide-react";
import { Button } from "@/components/ui/button";

interface NavItem {
  id: string;
  label: string;
  icon: React.ReactNode;
  href: string;
  roles: ("admin" | "teacher" | "student")[];
}

const allNavItems: NavItem[] = [
  // Admin Pages
  { id: "dashboard", label: "Панель управления", icon: <Home size={20} />, href: "/", roles: ["admin"] },
  { id: "users", label: "Пользователи", icon: <Users size={20} />, href: "/users", roles: ["admin"] },
  { id: "groups", label: "Группы", icon: <UsersIcon size={20} />, href: "/groups", roles: ["admin"] },
  { id: "disciplines", label: "Дисциплины", icon: <BookOpen size={20} />, href: "/disciplines", roles: ["admin"] },
  { id: "competencies", label: "Компетенции", icon: <Target size={20} />, href: "/competencies", roles: ["admin"] },
  { id: "indicators", label: "Индикаторы", icon: <Pin size={20} />, href: "/indicators", roles: ["admin"] },
  { id: "professions", label: "Профессии", icon: <Briefcase size={20} />, href: "/professions", roles: ["admin"] },
  // Teacher Pages
  { id: "teacher-disciplines", label: "Мои дисциплины", icon: <BookOpen size={20} />, href: "/teacher/disciplines", roles: ["teacher"] },
  // Student Pages
  { id: "student-profile", label: "Мой профиль", icon: <Users size={20} />, href: "/student/profile", roles: ["student"] },
  { id: "student-disciplines", label: "Дисциплины и оценки", icon: <BookOpen size={20} />, href: "/student/disciplines", roles: ["student"] },
  { id: "student-competencies", label: "Освоение компетенций", icon: <Target size={20} />, href: "/student/competencies", roles: ["student"] },
  { id: "student-professions", label: "Мои Профессии", icon: <Briefcase size={20} />, href: "/student/professions", roles: ["student"] },
];

export default function Sidebar() {
  const [location, setLocation] = useLocation();
  const [isOpen, setIsOpen] = useState(false);
  const [isCollapsed, setIsCollapsed] = useState(false);
  const userRole = sessionStorage.getItem("userRole") as "admin" | "teacher" | "student" | null;

  const isActive = (href: string) => location === href;
  
  const handleLogout = () => {
    sessionStorage.removeItem("userRole");
    sessionStorage.removeItem("userName");
    setLocation("/login");
  };

  const navItems = allNavItems.filter(item => userRole && item.roles.includes(userRole));

  return (
    <>
      {/* Mobile Menu Button */}
      <div className="fixed top-4 left-4 z-50 lg:hidden">
        <Button
          variant="outline"
          size="icon"
          onClick={() => setIsOpen(!isOpen)}
          className="bg-white"
        >
          {isOpen ? <X size={20} /> : <Menu size={20} />}
        </Button>
      </div>

      {/* Sidebar Overlay for Mobile */}
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-30 lg:hidden"
          onClick={() => setIsOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed left-0 top-0 h-screen bg-sidebar border-r border-sidebar-border transition-all duration-300 z-40 ${
          isCollapsed ? "w-20" : "w-60"
        } ${isOpen ? "translate-x-0" : "-translate-x-full lg:translate-x-0"}`}
      >
        {/* Logo Area */}
        <div className="h-20 flex items-center justify-center border-b border-sidebar-border px-4">
          <div className="text-center">
            <div className="text-xl font-bold text-sidebar-primary">CP</div>
            {!isCollapsed && <div className="text-xs text-sidebar-foreground/60">{userRole === "admin" ? "Администратор" : userRole === "teacher" ? "Преподаватель" : userRole === "student" ? "Студент" : "Профиль"}</div>}
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto py-4">
          {navItems.map((item) => (
            <Link 
              key={item.id} 
              href={item.href}
              onClick={() => setIsOpen(false)}
              className={`flex items-center gap-3 px-4 py-3 mx-2 rounded-lg transition-all relative ${
                isActive(item.href)
                  ? "bg-sidebar-accent text-sidebar-accent-foreground"
                  : "text-sidebar-foreground hover:bg-sidebar-accent/50"
              }`}
            >
              {isActive(item.href) && (
                <div className="absolute left-0 top-0 bottom-0 w-1 bg-sidebar-primary rounded-r-full" />
              )}
              <span className="flex-shrink-0">{item.icon}</span>
              {!isCollapsed && <span className="text-sm font-medium">{item.label}</span>}
            </Link>
          ))}
        </nav>

        {/* Logout Button */}
        <div className="border-t border-sidebar-border p-4">
          <button
            onClick={handleLogout}
            className={`flex items-center gap-3 w-full px-4 py-3 rounded-lg text-sidebar-foreground hover:bg-sidebar-accent/50 transition-all ${
              isCollapsed ? "justify-center" : ""
            }`}
          >
            <LogOut size={20} />
            {!isCollapsed && <span className="text-sm font-medium">Выход</span>}
          </button>
        </div>

        {/* Collapse Toggle */}
        <div className="hidden lg:flex justify-center p-2 border-t border-sidebar-border">
          <Button
            variant="ghost"
            size="sm"
            onClick={() => setIsCollapsed(!isCollapsed)}
            className="text-sidebar-foreground"
          >
            {isCollapsed ? "→" : "←"}
          </Button>
        </div>
      </aside>

      {/* Spacer for Desktop */}
      <div className={`hidden lg:block ${isCollapsed ? "w-20" : "w-60"} transition-all duration-300`} />
    </>
  );
}
