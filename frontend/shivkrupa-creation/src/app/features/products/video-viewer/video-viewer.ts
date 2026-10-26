import { Component, input, output, signal, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductVideo } from '../../../core/models/product.model';
import { ProductService } from '../../../core/services/product.service';

@Component({
  selector: 'app-video-viewer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './video-viewer.html',
  styleUrls: ['./video-viewer.css']
})
export class videoViewerComponent implements OnInit, OnDestroy {

  readonly productService = inject(ProductService);
  videos = input.required<ProductVideo[]>();
  activeIndex = input<number>(0);
  closeModal = output<void>();

  currentIndex = signal<number>(0);

  private segmentInterval: any = null;

  ngOnInit() {
    const currentvideos = this.videos();
    const initialIdx = this.activeIndex() || 0;

    if (currentvideos && currentvideos.length > 0) {
      const safeIdx = Math.min(Math.max(0, initialIdx), currentvideos.length - 1);
      this.currentIndex.set(safeIdx);

    }
  }

  nextvideo() {
    const total = this.videos().length;
    const nextIdx = (this.currentIndex() + 1) % total;
    this.currentIndex.set(nextIdx);

  }

  prevvideo() {
    const total = this.videos().length;
    const prevIdx = (this.currentIndex() - 1 + total) % total;
    this.currentIndex.set(prevIdx);
  }

  onClose() {
    this.clearTimer();
    this.closeModal.emit();
  }

  private clearTimer() {
    if (this.segmentInterval) {
      clearInterval(this.segmentInterval);
      this.segmentInterval = null;
    }
  }

  resolvevideo(url?: string | null): string {
    return this.productService.resolveMediaUrl(url);
  }

  ngOnDestroy() {
    this.clearTimer();
  }
}