import { User, LogOut } from "lucide-react";

interface HeaderProps {
  title: string;
  userName?: string;
  userRole?: string;
}

export default function Header({ title, userName = "Администратор", userRole = "admin" }: HeaderProps) {
  const getRoleLabel = (role: string) => {
    const roleMap: Record<string, string> = {
      admin: "Администратор",
      teacher: "Преподаватель",
      student: "Студент",
    };
    return roleMap[role] || role;
  };

  return (
    <header className="fixed top-0 left-0 right-0 h-20 bg-white border-b border-border flex items-center justify-between px-6 z-30 lg:ml-60">
      <h1 className="text-2xl font-bold text-foreground">{title}</h1>

      <div className="flex items-center gap-4">
        {/* User Profile */}
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-gradient-to-br from-primary to-sidebar-primary flex items-center justify-center text-white">
            <User size={20} />
          </div>
          <div className="hidden sm:block">
            <p className="text-sm font-semibold text-foreground">{userName}</p>
            <p className="text-xs text-muted-foreground">{getRoleLabel(userRole)}</p>
          </div>
        </div>

        {/* Status Indicator */}
        <div className="flex items-center gap-2">
          <div className="w-2 h-2 rounded-full bg-green-500" />
          <span className="text-xs text-muted-foreground hidden sm:inline">В сети</span>
        </div>

        {/* Logout Button */}
      </div>
    </header>
  );
}
