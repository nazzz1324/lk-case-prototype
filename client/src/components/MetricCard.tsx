import React from "react";

interface MetricCardProps {
  icon: React.ReactNode;
  value: number;
  label: string;
  color?: "blue" | "purple" | "green" | "orange";
}

const colorMap = {
  blue: "bg-blue-50 text-blue-600",
  purple: "bg-purple-50 text-purple-600",
  green: "bg-green-50 text-green-600",
  orange: "bg-orange-50 text-orange-600",
};

export default function MetricCard({ icon, value, label, color = "blue" }: MetricCardProps) {
  return (
    <div className="bg-card rounded-2xl p-6 shadow-sm border border-border hover:shadow-md transition-shadow">
      <div className={`w-12 h-12 rounded-lg ${colorMap[color]} flex items-center justify-center mb-4`}>
        {icon}
      </div>
      <div className="text-3xl font-bold text-foreground mb-1">{value}</div>
      <div className="text-sm text-muted-foreground">{label}</div>
    </div>
  );
} 
