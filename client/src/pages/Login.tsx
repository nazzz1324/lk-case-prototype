import { useState } from "react";
import { useLocation } from "wouter";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Lock, User } from "lucide-react";
import { api } from "@/lib/api";

export default function Login() {
  const [, setLocation] = useLocation();
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [loginError, setLoginError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoginError(null);
    setIsLoading(true);

    try {
      // В C# бэкенде LoginUserDto ожидает Login и Password
      const data = await api.auth.login({ login, password });
      
      sessionStorage.setItem("token", data.accessToken);
      
      // Определяем роль. В идеале она должна приходить в токене или отдельным полем.
      // Если бэкенд не возвращает роль, используем логику по умолчанию или декодируем JWT.
      const role = data.role || (login === "admin" ? "admin" : login === "teacher" ? "teacher" : "student");
      const userId = data.userId || 1;
      
      sessionStorage.setItem("userRole", role);
      sessionStorage.setItem("userId", userId.toString());
      sessionStorage.setItem("userName", login);
      
      if (role === "admin") {
        setLocation("/");
      } else if (role === "teacher") {
        setLocation("/teacher/disciplines");
      } else {
        setLocation("/student/profile");
      }
    } catch (error: any) {
      setLoginError(error.message || "Неверный логин или пароль.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary/10 via-background to-secondary flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="w-16 h-16 rounded-2xl bg-gradient-to-br from-primary to-sidebar-primary flex items-center justify-center mx-auto mb-4">
            <span className="text-2xl font-bold text-white">CP</span>
          </div>
          <h1 className="text-3xl font-bold text-foreground mb-2">Цифровой профиль</h1>
          <p className="text-muted-foreground">Система управления компетенциями студентов</p>
        </div>

        <div className="bg-card rounded-2xl shadow-lg p-8 border border-border">
          <form onSubmit={handleLogin} className="space-y-6">
            {loginError && (
              <div className="p-3 text-sm text-red-700 bg-red-100 border border-red-200 rounded-lg" role="alert">
                {loginError}
              </div>
            )}
            <div className="space-y-2">
              <Label htmlFor="login">Логин</Label>
              <div className="relative">
                <User className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <Input
                  id="login"
                  type="text"
                  placeholder="Введите ваш логин"
                  value={login}
                  onChange={(e) => setLogin(e.target.value)}
                  className="pl-10"
                  required
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="password">Пароль</Label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <Input
                  id="password"
                  type="password"
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="pl-10"
                  required
                />
              </div>
            </div>

            <Button
              type="submit"
              disabled={isLoading}
              className="w-full bg-primary hover:bg-primary/90 text-primary-foreground font-semibold py-2 rounded-lg transition-colors"
            >
              {isLoading ? "Вход..." : "Войти"}
            </Button>
          </form>
        </div>
      </div>
    </div>
  );
}
