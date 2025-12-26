import { useState } from "react";
import { useLocation } from "wouter";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { User, Lock, Loader2 } from "lucide-react";
import axios from "axios";

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
      const response = await axios.post("http://localhost:5041/api/Auth/login", {
        login: login,
        password: password
      });

      // Детальное логирование ответа сервера
      console.group("=== ДАННЫЕ ОТ СЕРВЕРА ===");
      console.log("Полный ответ:", response);
      console.log("response.data:", response.data);
      console.log("response.data.isSuccess:", response.data.isSuccess);
      console.log("response.data.data:", response.data.data);
      console.log("response.data.message:", response.data.message);
      
      if (response.data.data) {
        console.log("Ключи в data.data:", Object.keys(response.data.data));
        // Выводим все поля data.data
        for (const [key, value] of Object.entries(response.data.data)) {
          console.log(`data.data.${key}:`, value);
        }
      }
      console.groupEnd();

      if (response.data.isSuccess) {
        const data = response.data.data;
        
        // Теперь мы видим структуру данных в консоли
        // Адаптируем код под вашу структуру
        
        // Извлекаем роль - смотрим, в каком поле она находится
        let userRole = data.role || 
                      data.userRole || 
                      data.roleName || 
                      data.userType ||
                      data.type ||
                      "Student"; // Значение по умолчанию
        
        console.log("Определенная роль:", userRole);
        
        // Нормализуем роль
        const roleMap: Record<string, string> = {
          "Admin": "admin",
          "Administrator": "admin",
          "Teacher": "teacher", 
          "Educator": "teacher",
          "Student": "student",
          "Learner": "student",
          "Администратор": "admin",
          "Преподаватель": "teacher",
          "Студент": "student",
          "admin": "admin",
          "teacher": "teacher",
          "student": "student"
        };
        
        const normalizedRole = roleMap[userRole] || 
                             userRole.toLowerCase() || 
                             "student";
        
        console.log("Нормализованная роль:", normalizedRole);
        
        // ВАЖНО: Сохраняем userId - добавлено сохранение ID пользователя
        // Ищем ID в различных возможных полях
        const userId = data.id || 
                      data.userId || 
                      data.userID || 
                      data.Id || 
                      data.UserId ||
                      "";
        
        console.log("Найденный userId:", userId);
        
        // Сохраняем все данные в sessionStorage
        sessionStorage.setItem("token", data.token || data.accessToken || "");
        sessionStorage.setItem("userRole", normalizedRole);
        sessionStorage.setItem("userName", data.name || data.userName || data.fullName || data.login || login);
        sessionStorage.setItem("userId", userId); // ← ДОБАВЛЕНО сохранение ID
        sessionStorage.setItem("userLogin", login); // ← Дополнительно сохраняем логин
        
        // Для совместимости - также в localStorage если нужно
        localStorage.setItem("userId", userId);
        
        // Сохраняем все данные для отладки
        sessionStorage.setItem("userData", JSON.stringify(data));
        
        // Логируем сохраненные данные для проверки
        console.log("Сохраненные данные:", {
          userId: sessionStorage.getItem("userId"),
          userRole: sessionStorage.getItem("userRole"),
          userName: sessionStorage.getItem("userName"),
          token: sessionStorage.getItem("token") ? "Есть" : "Нет"
        });
        
        // Редирект в зависимости от роли
        if (normalizedRole === "admin") {
          setLocation("/");
        } else if (normalizedRole === "teacher") {
          setLocation("/teacher/disciplines");
        } else {
          setLocation("/student/profile");
        }
        
      } else {
        setLoginError(response.data.message || "Ошибка входа");
      }
    } catch (error: any) {
      console.error("Login error details:", error);
      
      if (error.response) {
        // Сервер вернул ошибку
        console.error("Error response data:", error.response.data);
        
        const errorData = error.response.data;
        
        if (errorData && errorData.message) {
          setLoginError(errorData.message);
        } else if (errorData && errorData.title) {
          setLoginError(errorData.title);
        } else if (errorData && typeof errorData === 'string') {
          setLoginError(errorData);
        } else {
          setLoginError(`Ошибка ${error.response.status}: ${error.response.statusText}`);
        }
      } else if (error.request) {
        setLoginError("Сервер не отвечает. Проверьте подключение.");
      } else {
        setLoginError("Ошибка при отправке запроса");
      }
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
              <Label htmlFor="login" className="text-sm font-medium">
                Логин
              </Label>
              <div className="relative">
                <User className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <Input
                  id="login"
                  type="text"
                  placeholder="Введите логин"
                  value={login}
                  onChange={(e) => setLogin(e.target.value)}
                  className="pl-10"
                  required
                  disabled={isLoading}
                />
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="password" className="text-sm font-medium">
                Пароль
              </Label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <Input
                  id="password"
                  type="password"
                  placeholder="Введите пароль"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="pl-10"
                  required
                  disabled={isLoading}
                />
              </div>
            </div>

            <Button
              type="submit"
              className="w-full bg-primary hover:bg-primary/90 text-primary-foreground font-semibold py-2 rounded-lg transition-colors"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Загрузка...
                </>
              ) : (
                "Войти"
              )}
            </Button>

            <div className="text-center text-xs text-muted-foreground">
              <p>После входа проверьте консоль браузера (F12) для просмотра структуры данных</p>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}