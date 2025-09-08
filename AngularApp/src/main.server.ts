import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { config } from './app/app.config.server';
import 'zone.js';

const bootstrap = () => bootstrapApplication(App, config);

export default bootstrap;
