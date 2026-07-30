import os
import csv
from pathlib import Path

def read_file(filepath):
    try:
        with open(filepath, 'rb') as f:
            data = f.read(6)
            header = data[:4].decode('utf-8', errors='replace')
            version = data[4]
            subversion = data[5]
                
        return header, version, subversion
    
    except Exception as e:
        print(f"Error processing {filepath}: {e}")
        return '', None, None

def scan_directory(root_path, output_csv):
    root = Path(root_path)
    
    results = []
    
    for filepath in root.rglob('*'):
        if filepath.is_file():
            relative_path = filepath.relative_to(root)
            
            extension = filepath.suffix
            
            header, version, subversion = read_file(filepath)
            
            results.append({
                'path': str(relative_path),
                'extension': extension,
                'header': header,
                'version': version,
                'subversion': subversion
            })
            
            print(f"Processing: {relative_path}")
    
    with open(output_csv, 'w', newline='', encoding='utf-8') as csvfile:
        fieldnames = ['path', 'extension', 'header', 'version', 'subversion']
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        
        writer.writeheader()
        writer.writerows(results)
    
    print(f"\n✓ Finished. {len(results)} processed files.")
    print(f"✓ CSV generated in: {output_csv}")

if __name__ == "__main__":
    unpack_path = "/.../Unpack_jp" # TODO send by parameter
    
    output_file = "jus_formats.csv"
    
    scan_directory(unpack_path, output_file)