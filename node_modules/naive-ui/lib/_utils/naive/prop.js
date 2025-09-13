"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.largerSize = largerSize;
exports.smallerSize = smallerSize;
function largerSize(size) {
    switch (size) {
        case 'tiny':
            return 'small';
        case 'small':
            return 'medium';
        case 'medium':
            return 'large';
        case 'large':
            return 'huge';
    }
}
function smallerSize(size) {
    switch (size) {
        case 'tiny':
            return 'mini';
        case 'small':
            return 'tiny';
        case 'medium':
            return 'small';
        case 'large':
            return 'medium';
        case 'huge':
            return 'large';
    }
    throw new Error(`${size} has no smaller size.`);
}
