import { Component, Input } from '@angular/core';

@Component({
  selector: 'sc-empty-state',
  standalone: true,
  template: `
    <div class="text-center py-5">
      <i class="bi {{ icon }} display-4 text-muted mb-3 d-block"></i>
      <h5 class="mb-2">{{ title }}</h5>
      @if (subtitle) {
        <p class="text-muted mb-0">{{ subtitle }}</p>
      }
      <ng-content></ng-content>
    </div>
  `
})
export class EmptyStateComponent {
  @Input() icon = 'bi-inbox';
  @Input() title = 'Nothing here yet';
  @Input() subtitle = '';
}
