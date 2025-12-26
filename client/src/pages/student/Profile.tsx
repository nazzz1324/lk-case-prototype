import { useState, useEffect } from "react";
import { User, Mail, BookOpen, Loader2 } from "lucide-react";
import { apiClient } from "@/lib/api";
import { toast } from "sonner";

interface StudentData {
  id: number;
  fullname: string;
  email: string;
  role: string;
  isActive: boolean;
}

export default function StudentProfile() {
  const [profile, setProfile] = useState<StudentData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        setLoading(true);
        const userId = sessionStorage.getItem("userId");
        if (!userId) {
          toast.error("ID пользователя не найден. Войдите снова.");
          return;
        }

        const response = await apiClient.getUsers();
        if (response.data && Array.isArray(response.data)) {
          const currentId = parseInt(userId);
          const currentUser = response.data.find((u: any) => 
            (u.id === currentId) || (u.userId === currentId) || (u.userID === currentId)
          );

          if (currentUser) {
            setProfile(currentUser);
          } else {
            toast.error("Данные пользователя не найдены");
          }
        }
      } catch (error) {
        console.error("Failed to fetch profile:", error);
        toast.error("Не удалось загрузить данные профиля");
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
        <span className="ml-2 text-lg text-muted-foreground">Загрузка профиля...</span>
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="p-8 text-center text-muted-foreground">
        Данные профиля не найдены.
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <div className="bg-card rounded-2xl p-8 shadow-sm border border-border">
        <div className="flex flex-col sm:flex-row items-start sm:items-center gap-6">
          <div className="w-24 h-24 rounded-2xl bg-gradient-to-br from-primary to-sidebar-primary flex items-center justify-center text-white shadow-lg">
            <User size={48} />
          </div>

          <div className="flex-1">
            <h1 className="text-3xl font-bold text-foreground mb-2">{profile.fullname}</h1>
            <div className="space-y-2 text-muted-foreground">
              <div className="flex items-center gap-2">
                <Mail size={16} className="text-primary" />
                <span>{profile.email}</span>
              </div>
              <div className="flex items-center gap-2">
                <BookOpen size={16} className="text-primary" />
                <span>Группа: </span>
              </div>
              <div className="flex items-center gap-2">
                <User size={16} className="text-primary" />
                <span>Роль: {profile.role}</span>
              </div>
            </div>
          </div>

          <div className="text-right">
            <div className={`inline-flex items-center gap-2 px-4 py-2 rounded-full text-sm font-medium ${
              profile.isActive ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"
            }`}>
              <div className={`w-2 h-2 rounded-full ${profile.isActive ? "bg-green-600" : "bg-red-600"}`} />
              {profile.isActive ? "Активен" : "Неактивен"}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
