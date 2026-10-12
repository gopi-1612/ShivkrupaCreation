import { Component, Input } from '@angular/core';

@Component({
  selector: 'sc-loading-spinner',
  standalone: true,
  template: `
    <div class="sc-loading d-flex flex-column align-items-center justify-content-center py-5">
      <div class="spinner-border" role="status" style="color: var(--sc-maroon);">
        <span class="visually-hidden">Loading…</span>
      </div>
      @if (message) {
        <p class="mt-3 text-muted mb-0">{{ message }}</p>
      }
    </div>
  `
})
export class LoadingSpinnerComponent {
  @Input() message = 'Loading…';
}
