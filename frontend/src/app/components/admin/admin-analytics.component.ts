import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

import Chart from 'chart.js/auto';

@Component({
  selector: 'app-admin-analytics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-analytics.component.html',
  styleUrls: ['./admin-analytics.component.css']
})
export class AdminAnalyticsComponent implements OnInit, AfterViewInit {
  @ViewChild('vehicleCanvas', { static: false }) vehicleCanvas!: ElementRef<HTMLCanvasElement>;

  vehicleChart: any = null;

  stats = { totalRequests: 0 };
  statusCounts: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    // fetch simple totals for header - we only need the counts for cards
    this.http.get<any>(`${environment.apiBaseUrl}/api/admin/analytics/status-counts`).subscribe({
      next: (data) => {
        this.statusCounts = data || [];
        this.stats.totalRequests = this.statusCounts.reduce((acc: number, i: any) => acc + (i.count || 0), 0);
      },
      error: () => { /* fail silently */ }
    });

    this.http.get<any>(`${environment.apiBaseUrl}/api/admin/analytics/vehicle-type-counts`).subscribe({
      next: (data) => this.buildVehicleChart(data),
      error: () => { /* fail silently */ }
    });
  }

  ngAfterViewInit() {
    // charts will be built after data arrives
  }

  // status cards are displayed directly; chart removed per UI request

  buildVehicleChart(data: any[]) {
    try {
      const labels = data.map(d => d.type);
      const counts = data.map(d => d.count);
      if (this.vehicleCanvas) {
        const ctx = this.vehicleCanvas.nativeElement.getContext('2d') as CanvasRenderingContext2D;
        if (this.vehicleChart) this.vehicleChart.destroy();
        // Use a bar chart for vehicles by type with distinct colors
        const colors = ['#ef4444', '#f59e0b', '#10b981', '#3b82f6', '#8b5cf6', '#ec4899', '#06b6d4'];
        this.vehicleChart = new Chart(ctx, {
          type: 'bar',
          data: {
            labels,
            datasets: [{ label: 'Vehicles', data: counts, backgroundColor: labels.map((_, i) => colors[i % colors.length]) }]
          },
          options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
        });
      }
    } catch (e) { console.error('Failed to build vehicle chart', e); }
  }
}
