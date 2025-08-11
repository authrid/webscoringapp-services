// app/applications/page.tsx
import React from 'react';

type Application = {
  id: number;
  appNo: string;
  totalScore: number;
  riskCategoryName: string;
};

async function getApplications(): Promise<Application[]> {
  const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/application`, {
    cache: 'no-store', // agar selalu fetch data terbaru
  });
  if (!res.ok) {
    throw new Error("Gagal memuat data");
  }
  return res.json();
}

export default async function ApplicationsPage() {
  const apps = await getApplications();

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Daftar Applications</h1>
      <table className="border border-gray-300 w-full">
        <thead>
          <tr className="bg-gray-100">
            <th className="border p-2">ID</th>
            <th className="border p-2">App No</th>
            <th className="border p-2">Total Score</th>
            <th className="border p-2">Risk Category</th>
          </tr>
        </thead>
        <tbody>
          {apps.map((app) => (
            <tr key={app.id}>
              <td className="border p-2">{app.id}</td>
              <td className="border p-2">{app.appNo}</td>
              <td className="border p-2">{app.totalScore}</td>
              <td className="border p-2">{app.riskCategoryName}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
