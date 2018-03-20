from distutils.core import setup

setup(
    name='treehopper',
    version='1.11.2',
    packages=['treehopper.api','treehopper.libraries', 'treehopper.utils'],
    license='MIT license',
    description='Treehopper USB boards connect the physical world to your computer, smartphone, or tablet.',
    long_description=open('README.txt').read(),
    install_requires=[
        'pyusb'
    ]
)
