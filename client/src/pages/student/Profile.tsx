import { User, Mail, BookOpen } from "lucide-react";
import { useState, useEffect } from "react";
import { api } from "@/lib/api";

export default function StudentProfile() {
  const [profile, setProfile] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const studentId = parseInt(sessionStorage.getItem("userId") || "1");

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const data = await api.student.getProfile(studentId);
        setProfile(data);
      } catch (error) {
        console.error("Failed to fetch student profile:", error);
        // Fallback to session storage if API fails
        setProfile({
          name: sessionStorage.getItem("userName") || "Студент",
          email: "student@university.ru",
          group: "ПИ-21-1",
          status: "active"
        });
      } finally {
        setIsLoading(false);
      }
    };
    fetchProfile();
  }, [studentId]);

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="space-y-8">
      <div className="bg-card rounded-2xl p-8 shadow-sm border border-border">
        <div className="flex flex-col sm:flex-row items-start sm:items-center gap-6">
          <div className="w-24 h-24 rounded-2xl bg-gradient-to-br from-primary to-sidebar-primary flex items-center justify-center text-white">
            <User size={48} />
          </div>

          <div className="flex-1">
            <h1 className="text-3xl font-bold text-foreground mb-2">{profile?.name}</h1>
            <div className="space-y-2 text-muted-foreground">
              <div className="flex items-center gap-2">
                <Mail size={16} />
                <span>{profile?.email}</span>
              </div>
              <div className="flex items-center gap-2">
                <BookOpen size={16} />
                <span>Группа: {profile?.group}</span>
              </div>
            </div>
          </div>

          <div className="text-right">
            <div className={`inline-flex items-center gap-2 px-4 py-2 rounded-full text-sm font-medium ${profile?.status === "active" ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700"}`}>
              <div className={`w-2 h-2 rounded-full ${profile?.status === "active" ? "bg-green-600" : "bg-red-600"}`} />
              {profile?.status === "active" ? "Активен" : "Заблокирован"}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
