import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [],
  template: `
    <div class="spinner-overlay" *ngIf="loading">
      <div class="spinner"></div>
    </div>
  `,
  styleUrl: './spinner.component.scss'
})
export class SpinnerComponent {
  @Input() loading = false;
}
