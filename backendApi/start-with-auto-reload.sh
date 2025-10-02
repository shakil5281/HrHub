#!/bin/bash
echo "Starting HrHub API with Auto-Reload..."
echo ""
echo "This will automatically restart the application when you make code changes."
echo "Press Ctrl+C to stop the application."
echo ""
dotnet watch --verbose
