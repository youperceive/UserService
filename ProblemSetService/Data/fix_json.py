import json

# 读取JSON文件
with open('Questions.json', 'r', encoding='utf-8') as f:
    questions = json.load(f)

fixed_count = 0
total_questions = len(questions)

# 遍历所有题目
for question in questions:
    if 'examples' in question:
        for example in question['examples']:
            if 'output' in example:
                # 如果output是数组，转换为字符串
                if isinstance(example['output'], list):
                    # 将数组元素用换行符或逗号分隔连接成字符串
                    example['output'] = ', '.join(str(item) for item in example['output'])
                    fixed_count += 1
                    print(f"修复题目 {question['id']}: {question['title']}")

# 保存修复后的JSON
with open('Questions.json', 'w', encoding='utf-8') as f:
    json.dump(questions, f, ensure_ascii=False, indent=2)

print(f"\n修复完成！")
print(f"总题目数: {total_questions}")
print(f"修复的output字段数: {fixed_count}")

